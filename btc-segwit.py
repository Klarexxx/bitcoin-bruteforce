from mnemonic import Mnemonic
from bip32utils import BIP32Key, BIP32_HARDEN
import hashlib
import bech32
import mysql.connector
from threading import Thread
from pystyle import *
from time import sleep




th = input("Thread -> ")




tot = int(th) - int(th) - int(th)
total = tot





def main():

    global total


    mydb = mysql.connector.connect(
    host="",
    user="",
    password="",
    database="address",
    )

    mycursor = mydb.cursor()


    while True:
        total+=1

        mnemon = Mnemonic('english')
        words = mnemon.generate(128)



        seed = Mnemonic().to_seed(words)


        bip32_master_key = BIP32Key.fromEntropy(seed)

        # BIP84 path: m/84'/0'/0'/0/0
        bip32_child_key = bip32_master_key.ChildKey(
            84 + BIP32_HARDEN).ChildKey(
            0 + BIP32_HARDEN).ChildKey(
            0 + BIP32_HARDEN).ChildKey(0).ChildKey(0)


        def pubkey_to_bech32_address(pubkey):
            pubkey_hash = hashlib.new('ripemd160', hashlib.sha256(pubkey).digest()).digest()
            return bech32.encode('bc', 0, pubkey_hash)


        public_key = bip32_child_key.PublicKey()

        address = pubkey_to_bech32_address(public_key)


        mycursor.execute(f"SELECT * FROM `address` WHERE address = \"{address}\"")
        myresult = mycursor.fetchall()



        if myresult == []:
            print(Colors.red+"Try: "+str(address)+" Balance 0")
        else:
            print(Colors.green+"Found: "+str(address))
            file = open("bitcoin.txt","a+")
            wif = bip32_child_key.WalletImportFormat()
            file.write("Address: "+str(address)+" Words: "+str(words)+" Wif: "+str(wif)+"\n")
            print(wif)
            file.close()
            input()




        #private_key = bip32_child_key.WalletImportFormat()
        #print("Address:", address)
        #print("Private Key:", private_key)










for c in range(int(th)):
    t1 = Thread(target=main)
    t1.start()

def count():
    while True:
        System.Title("Check Wallet: "+str(total))
        sleep(1)


t2 = Thread(target=count)
t2.start()
