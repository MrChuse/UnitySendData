import socket
import threading
import time

PORT = 9002
IP = '' # all ips
POSSIBLE_CONNECTIONS = 1
CHUNK = 4096

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
            
    def receive(self, conn, img_len, chunk):
        read_by_now = 0
        recieved_bytes = b''
        while read_by_now < img_len - chunk:
            data = conn.recv(chunk)
            if not data:
                break
            recieved_bytes += data
            read_by_now += chunk
        data = conn.recv(img_len - read_by_now)
        recieved_bytes += data
        return recieved_bytes
            
    def handle_connection(self, num, conn, addr):
        print('Connection', num, ':', 'established')
        
        print('Connection', num, ':', 'established')
        recieve = int.from_bytes(conn.recv(4), 'little')
        print('Connection', num, ':', recieve)
        while recieve:
            list_size = int.from_bytes(conn.recv(4), 'little')
            print('Connection', num, ':', list_size, 'bytes to receive')
            
            list_bytes = self.receive(conn, list_size, CHUNK)
            print('Connection', num, ':', 'received data')
            
            values = [int.from_bytes(t, 'little', signed=True) for t in zip(*[iter(list_bytes)]*4)]
            
            # do something with data
            # e.g. print it
            print(values, type(values[0]))
            
            recieve = int.from_bytes(conn.recv(4), 'little')
            print('Connection', num, ':', recieve)
        
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
