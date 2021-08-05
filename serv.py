import socket
import threading
import time
import struct

PORT = 9003
IP = '' # all ips
POSSIBLE_CONNECTIONS = 1
CHUNK = 4096
#convert = [None, int.from_bytes, float, bool, str]
convert = ' if?'
def get_format(typ, leng):
    if typ != 4:
        return convert[typ]
    return str(leng) + 's'


class Server:
    def __init__(self, ip, port):
        self.sock = socket.socket()
        self.sock.bind((ip, port))
        self.sock.listen(POSSIBLE_CONNECTIONS)
        print('Server Initialized')
        self.start_listener()
    
    def start_listener(self):
        try:
            connections = 0
            while True:
                print('Waiting for a connection...')
                conn, addr = self.sock.accept()
                connections += 1
                print('Connected!')
                t = threading.Thread(target=self.handle_connection, args=(connections, conn, addr), daemon=True)
                t.start()
        except TypeError as e:
            print('Exception:', e)
            
    # def receive(self, conn, img_len, chunk):
        # read_by_now = 0
        # recieved_bytes = b''
        # while read_by_now < img_len - chunk:
            # data = conn.recv(chunk)
            # if not data:
                # break
            # recieved_bytes += data
            # read_by_now += chunk
        # data = conn.recv(img_len - read_by_now)
        # recieved_bytes += data
        # return recieved_bytes
            
    def receive(self, conn, img_len, chunk):
        # wrapper function for conn.recv
        img_bytes = b''
        while len(img_bytes) < img_len:
            if img_len - len(img_bytes) < chunk:
                data = conn.recv(img_len - len(img_bytes))
            else:
                data = conn.recv(chunk)
            if not data:
                break
            img_bytes += data
            #print('in receive', len(img_bytes), 'data ->', data)
        return img_bytes
            
            
    def receive_int_list(self, num, conn):
        list_size = int.from_bytes(conn.recv(4), 'little')
        print('Connection', num, ':', list_size, 'bytes to receive')
        
        list_bytes = self.receive(conn, list_size, CHUNK)
        print('Connection', num, ':', 'received data')#, list_bytes)
        
        
        values = struct.unpack(str(list_size//4) + 'i', list_bytes)
        #values = [int.from_bytes(t, 'little', signed=True) for t in zip(*[iter(list_bytes)]*4)]
        return values
            
    
    def receive_frame_data(self, num, conn):
        types = self.receive_int_list(num, conn)
        lengths = self.receive_int_list(num, conn)
        data = self.receive(conn, sum(lengths), CHUNK)
        print('Connection', num, ':', data, types, lengths)
        s = '<'
        for typ, leng in zip(types, lengths):
            s += get_format(typ, leng)
        print('Connection', num, ': s ', s)
        l = struct.unpack(s, data)
        return l
            
    def handle_connection(self, num, conn, addr):
        print('Connection', num, ':', 'established')
        recieve = int.from_bytes(conn.recv(4), 'little')
        print('Connection', num, ': continue?', recieve)
        while recieve:
            values = self.receive_frame_data(num, conn)
            print('values', values)
            
            recieve = int.from_bytes(conn.recv(4), 'little')
            print('Connection', num, ': continue?', recieve)
            if recieve != 1:
                print('wow look over here :0 ' + '!' * 200)
        
        print('Connection', num, ':', 'stopped connection and thread')
        conn.close()
        
def thread_function():
    myserver = Server(IP, PORT)

def main():
    t = threading.Thread(target=thread_function, daemon=True)
    t.start()
    print('Server Started...!')
    while True:
        time.sleep(1)

if __name__ == '__main__':
    main()
