#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include "XL320.h"
#include <SoftwareSerial.h>
SoftwareSerial mySerial(5, 15); 

//      GPIO
// D0 = IO 16
// D1 = IO 5
// D2 = IO 4
// D3 = IO 0
// D4 = IO 2
// D5 = IO 14
// D6 = IO 12
// D7 = IO 13
// D8 = IO 15

#ifndef STASSID
#define STASSID "TP-Link_B052"
#define STAPSK "74135226"

#endif

#define PRINT_WIFI_STATUS

unsigned int localPort = 8052;
char packetBuffer[256];
WiFiUDP Udp;

XL320 robot;
char rgb[] = "rgbypcwo";
int servoID = 3;
int colorVal;
int defaultPosition = 512;
int servoPosition;
int MaxPos = 912;
int MinPos = 112;
int RotationStep = 400;

long waitTime = 3000;
long now;

void setup() {
  Serial.begin(115200);
  mySerial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.begin(STASSID, STAPSK);
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.print('.');
    delay(1000);
  }
  Udp.begin(localPort);
  robot.begin(mySerial);
  robot.LED(servoID, &rgb[5]); // white
  robot.setJointTorque(servoID, 1023);
  robot.setJointSpeed(servoID, 300);
  robot.moveJoint(servoID, defaultPosition);
  servoPosition = defaultPosition;
  now = millis();
}

void loop() {
  int packetSize = Udp.parsePacket();

  if (packetSize)
  {
    Udp.read(packetBuffer, 255);
    //    if (len > 0) {
    //      packetBuffer[len] = 0;
    //    }
    //    int message = atoi(packetBuffer);
    char message = packetBuffer[0];
    //    Serial.println(message);
    switch (message) {
      case 's':
        colorVal = 1; //green
        servoPosition = constrain(servoPosition, MinPos, MaxPos);
        Serial.println(servoPosition);
        break;

      case 'r':
        colorVal = 2; // blue
        servoPosition -= RotationStep;
        servoPosition = constrain(servoPosition, MinPos, MaxPos);
        Serial.println(servoPosition);
        break;

      case 'l':
        colorVal = 4; //purple
        servoPosition += RotationStep;
        servoPosition = constrain(servoPosition, MinPos, MaxPos);
        Serial.println(servoPosition);
        break;

      case 'x':
        colorVal = 0; //purple
        servoPosition = defaultPosition;
        servoPosition = constrain(servoPosition, MinPos, MaxPos);
        Serial.println(servoPosition);
        break;
    }
  }

#ifdef PRINT_WIFI_STATUS
  if (millis() - now >= waitTime) {
    printWifiStatus();
    now = millis();
  }
#endif

  robot.setJointTorque(servoID, 1023);
  robot.setJointSpeed(servoID, 100);
  robot.LED(servoID, &rgb[colorVal]);
  robot.moveJoint(servoID, servoPosition);
}

void printWifiStatus() {
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");


  uint8_t macAddr[6];
  WiFi.macAddress(macAddr);
  Serial.printf("Connected, mac address: %02x:%02x:%02x:%02x:%02x:%02x\n", macAddr[0], macAddr[1], macAddr[2], macAddr[3], macAddr[4], macAddr[5]);
}
