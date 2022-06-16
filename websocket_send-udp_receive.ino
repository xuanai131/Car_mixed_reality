#include "esp_camera.h"
#include <WiFi.h>
#include <ArduinoWebsockets.h>
#define CAMERA_MODEL_AI_THINKER
#include "camera_pins.h"
// Include the servo library:
#include <ESP32Servo.h>

#include "soc/timer_group_struct.h"
#include "soc/timer_group_reg.h"

#include "soc/soc.h"
#include "soc/rtc_cntl_reg.h"

const char* ssid = "xuanai@";
const char* password = "123456789";
const char* websocket_server_host = "192.168.191.103";
//const char* websocket_server_host = "192.168.1.7";
const uint16_t websocket_server_port = 8888;

const int listenport = 44444;

// Create a new servo object:
//Servo myservo1;
//Servo myservo2;

// Define the servo pin:
//#define servo1 13
//#define servo2 15

// Create a variable to store the servo position:
int angle1 = 0;
int angle2 = 0;

using namespace websockets;
WebsocketsClient client;
TaskHandle_t Task1;
String last_variable;
//create UDP instance
WiFiUDP udp;

//char variable[50];
//void control()
//{
//  if(!strcmp(variable, "forward")) {
//    Serial.println("Forward");
//    digitalWrite(12, 1);
//    digitalWrite(13, 0);
//    digitalWrite(15, 0);
//    digitalWrite(14, 0);
//  }
//  else if(!strcmp(variable, "left")) {
//    Serial.println("Left");
//    digitalWrite(12, 0);
//    digitalWrite(13, 1);
//    digitalWrite(15, 0);
//    digitalWrite(14, 0);
//  }
//  else if(!strcmp(variable, "right")) {
//    Serial.println("Right");
//    digitalWrite(12, 0);
//    digitalWrite(13, 0);
//    digitalWrite(15, 1);
//    digitalWrite(14, 0);
//  }
//  else if(!strcmp(variable, "backward")) {
//    Serial.println("backward");
//    digitalWrite(12, 0);
//    digitalWrite(13, 1);
//    digitalWrite(15, 1);
//    digitalWrite(14, 0);
//  }
//  else if(!strcmp(variable, "stop")) {
//    Serial.println("Stop");
//    digitalWrite(12, 0);
//    digitalWrite(13, 0);
//    digitalWrite(15, 0);
//    digitalWrite(14, 1);
//  }
//  else if(!strcmp(variable, "up")) {
//    Serial.println("Up");
//    digitalWrite(12, 1);
//    digitalWrite(13, 0);
//    digitalWrite(15, 0);
//    digitalWrite(14, 1);
//  }
//  else if(!strcmp(variable, "down")) {
//    Serial.println("Down");
//    digitalWrite(12, 0);
//    digitalWrite(13, 1);
//    digitalWrite(15, 0);
//    digitalWrite(14, 1);
//  }
//}


  
void setup() {
  WRITE_PERI_REG(RTC_CNTL_BROWN_OUT_REG, 0); //disable brownout 
  Serial.begin(115200);
  Serial.setDebugOutput(true);
  Serial.println();

  camera_config_t config;
  config.ledc_channel = LEDC_CHANNEL_0;
  config.ledc_timer = LEDC_TIMER_0;
  config.pin_d0 = Y2_GPIO_NUM;
  config.pin_d1 = Y3_GPIO_NUM;
  config.pin_d2 = Y4_GPIO_NUM;
  config.pin_d3 = Y5_GPIO_NUM;
  config.pin_d4 = Y6_GPIO_NUM;
  config.pin_d5 = Y7_GPIO_NUM;
  config.pin_d6 = Y8_GPIO_NUM;
  config.pin_d7 = Y9_GPIO_NUM;
  config.pin_xclk = XCLK_GPIO_NUM;
  config.pin_pclk = PCLK_GPIO_NUM;
  config.pin_vsync = VSYNC_GPIO_NUM;
  config.pin_href = HREF_GPIO_NUM;
  config.pin_sscb_sda = SIOD_GPIO_NUM;
  config.pin_sscb_scl = SIOC_GPIO_NUM;
  config.pin_pwdn = PWDN_GPIO_NUM;
  config.pin_reset = RESET_GPIO_NUM;
  config.xclk_freq_hz = 10000000;
  config.pixel_format = PIXFORMAT_JPEG;
  //init with high specs to pre-allocate larger buffers
  if(psramFound()){
    config.frame_size = FRAMESIZE_VGA;
    config.jpeg_quality = 10;
    config.fb_count = 2;
  } else {
    config.frame_size = FRAMESIZE_SVGA;
    config.jpeg_quality = 12;
    config.fb_count = 1;
  }


  // camera init
  esp_err_t err = esp_camera_init(&config);
  if (err != ESP_OK) {
    Serial.printf("Camera init failed with error 0x%x", err);
    return;
  }

  //pinMode(33, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(13, OUTPUT);
  pinMode(14, OUTPUT);
  pinMode(15, OUTPUT);
  pinMode(16, OUTPUT);
  pinMode(2, OUTPUT);
  pinMode(4, OUTPUT);
  // Attach the Servo variable to a pin:
//  myservo1.attach(servo1);
//  myservo2.attach(servo2);
  
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    }
  Serial.println("");
  Serial.println("WiFi connected");

  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
  udp.begin(listenport);
    
  while(!client.connect(websocket_server_host, websocket_server_port, "/")){
    delay(500);
    Serial.print(".");
  }
  Serial.println("Websocket Connected!");
  
  xTaskCreatePinnedToCore(
                    Task1code,   /* Task function. */
                    "Task1",     /* name of task. */
                    10000,       /* Stack size of task */
                    NULL,        /* parameter of the task */
                    1,           /* priority of the task */
                    &Task1,      /* Task handle to keep track of created task */
                    0);          /* pin task to core 0 */                  
  delay(500); 
  
}


void Task1code( void * pvParameters ){
  Serial.print("Task1 running on core ");
  Serial.println(xPortGetCoreID());

  for(;;){
    byte buffer[50] = {0};
    memset(buffer, 0, 50);
    //processing incoming packet, must be called before reading the buffer
    udp.parsePacket();
    //receive response from server, it will be HELLO WORLD
    int len = udp.read(buffer, 50);
    

      //Serial.println(len);
      String variable = (char*)buffer;
      /*if(variable=="forward") {
    Serial.println("F");
    digitalWrite(12, 0);
    digitalWrite(13, 0);
    digitalWrite(15, 0);
    digitalWrite(14, 1);
  }
  else if(variable=="left") {
    Serial.println("L");
    digitalWrite(12, 0);
    digitalWrite(13, 0);
    digitalWrite(15, 1);
    digitalWrite(14, 0);
  }
  else if(variable=="right") {
    Serial.println("R");
    digitalWrite(12, 1);
    digitalWrite(13, 0);
    digitalWrite(15, 0);
    digitalWrite(14, 0);
  }
  else if(variable=="backward") {
    Serial.println("B");
    digitalWrite(12, 1);
    digitalWrite(13, 0);
    digitalWrite(15, 1);
    digitalWrite(14, 0);
  }
  else if(variable=="stop") {
    Serial.println("S");
    digitalWrite(12, 0);
    digitalWrite(13, 1);
    digitalWrite(15, 0);
    digitalWrite(14, 0);
  }
  else if(variable=="up") {
    Serial.println("U");
    digitalWrite(12, 0);
    digitalWrite(13, 1);
    digitalWrite(15, 0);
    digitalWrite(14, 1);
  }
  else if(variable=="down") {
    Serial.println("D");
    digitalWrite(12, 0);
    digitalWrite(13, 1);
    digitalWrite(15, 1);
    digitalWrite(14, 0);
  }*/
 /* else if(variable=="") {
    digitalWrite(12, 0);
    digitalWrite(13, 0);
    digitalWrite(15, 0);
    digitalWrite(14, 0);
  }*/

  /////////////////////////////////////////
  if (len>0 && variable!=last_variable)
  {
    Serial.println(variable);
    if(variable == "forward") {
      digitalWrite(2, 0);
      digitalWrite(4, 1);
      digitalWrite(12, 0);
      digitalWrite(13, 0);
      digitalWrite(14, 0);
      digitalWrite(15, 0);
      digitalWrite(16, 0);
      Serial.println("Forward");
    }
    else if(variable == "left") {
      digitalWrite(2, 1);
      digitalWrite(4, 0);
      digitalWrite(12, 0);
      digitalWrite(13, 0);
      digitalWrite(14, 0);
      digitalWrite(15, 0);
      digitalWrite(16, 0);
      Serial.println("Left");
    }
    else if(variable == "right") {
      digitalWrite(2, 0);
      digitalWrite(4, 0);
      digitalWrite(12, 1);
      digitalWrite(13, 0);
      digitalWrite(14, 0);
      digitalWrite(15, 0);
      digitalWrite(16, 0);
      Serial.println("Right");
    }
    else if(variable == "backward") {
      digitalWrite(2, 0);
      digitalWrite(4, 0);
      digitalWrite(12, 0);
      digitalWrite(13, 1);
      digitalWrite(14, 0);
      digitalWrite(15, 0);
      digitalWrite(16, 0);
      Serial.println("Backward");
    }
    else 
    if(variable == "down") {
      digitalWrite(2, 0);
      digitalWrite(4, 0);
      digitalWrite(12, 0);
      digitalWrite(13, 0);
      digitalWrite(14, 1);
      digitalWrite(15, 0);
      digitalWrite(16, 0);
      Serial.println("Down");
    }
    else if(variable == "up") {
      digitalWrite(2, 0);
      digitalWrite(4, 0);
      digitalWrite(12, 0);
      digitalWrite(13, 0);
      digitalWrite(14, 0);
      digitalWrite(15, 1);
      digitalWrite(16, 0);
      Serial.println("Up");
    }
    else if(variable == "stop"){
      digitalWrite(2, 0);
      digitalWrite(4, 0);
      digitalWrite(12, 0);
      digitalWrite(13, 0);
      digitalWrite(14, 0);
      digitalWrite(15, 0);
      digitalWrite(16, 1);
      Serial.println("Stop");
    }
  }
  if (variable!="")
  {
    last_variable = variable;
  }
  }
  
  /////////////////////////////////
//      if (myString=="A") digitalWrite(33, HIGH);
//      if (myString=="B") digitalWrite(33, LOW);
      
//        int angle1 = (uint8_t)buffer[0];
//        Serial.println(angle1);
//        if (angle1>=0 && angle1<=90) myservo1.write(angle1);
//        int angle2 = (uint8_t)buffer[1];
//        Serial.println(angle2);
//        if (angle2>=0 && angle2<=90) myservo2.write(angle2);
//        Serial.println((uint8_t)buffer[2]);

    delay(1);
    //TIMERG0.wdt_wprotect=TIMG
    //_WDT_WKEY_VALUE;
    //TIMERG0.wdt_feed=1;
    //TIMERG0.wdt_wprotect=0;
  } 

void loop() {
  camera_fb_t *fb = esp_camera_fb_get();
  if(!fb){
    Serial.println("Camera capture failed");
    esp_camera_fb_return(fb);
    return;
  }

  if(fb->format != PIXFORMAT_JPEG){
    Serial.println("Non-JPEG data not implemented");
    return;
  }

  client.sendBinary((const char*) fb->buf, fb->len);
  esp_camera_fb_return(fb);
      TIMERG0.wdt_wprotect=TIMG_WDT_WKEY_VALUE;
      TIMERG0.wdt_feed=1;
      TIMERG0.wdt_wprotect=0;
  delay(1);
}
