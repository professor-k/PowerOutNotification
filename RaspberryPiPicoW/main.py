import network
import urequests as requests
import os
import json
from time import sleep
from picozero import pico_led
import machine

def getConfig():
    with open("config.json") as fd:
        return json.load(fd)

def connect(ssid, password):
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    wlan.connect(ssid, password)
    while wlan.isconnected() == False:
        print('Waiting for connection...')
        sleep(5)
        if wlan.isconnected() == False:
            wlan.connect(ssid, password)
    ip = wlan.ifconfig()[0]
    print(f'Connected on {ip}')

c = getConfig()
connect(c["ssid"], c["password"])
pico_led.on()
while True:
    try:
        // TODO: address has to be url encoded, due to https://github.com/micropython/micropython-lib/issues/395
        request = requests.get(f"{c["url"]}/powerOn?address={c["address"]}")
        print(request.content)
        request.close()
        sleep(10)
    except:
        machine.reset()

