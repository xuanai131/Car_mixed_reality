using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Android;
using TMPro;
using System.Text;
using UnityEngine.InputSystem;

public class send_to_esp : MonoBehaviour
{
    public Text text1;
    public SendUDP udp;
    // Start is called before the first frame update
    void Start()
    {
        text1.GetComponent<Text>();
        udp = new SendUDP();
    }

    // Update is called once per frame
    void Update()
    {
        //byte x = (byte)(transform.rotation.x * 180 + 60);   //120
        //byte y = (byte)(transform.rotation.y * 180 + 60);   //120
        //byte[] value = { x, y };
        //List<int> numbers = new List<int>();
        //text.text = x.ToString() + ", " + y.ToString();
        /*try
        {
            var gamepad = AndroidGamepad.current;
            if (gamepad.aButton.ReadValue() == 1.0f)
            {
                A = 1;
            }
            else if (gamepad.bButton.ReadValue() == 1.0f)
            {
                B = 1;
            }
            else if (gamepad.xButton.ReadValue() == 1f)
            {
                X = 1;
            }
            else if (gamepad.yButton.ReadValue() == 1f)
            {
                Y = 1;
            }
        }
        catch
        {
            Debug.Log(".......");
        }*/


        string content = "";
        try
        {
            var gamepad = AndroidGamepad.current;
            if (gamepad.aButton.ReadValue() == 1.0f)
            {
                content = "stop";
            }
            else if (gamepad.bButton.ReadValue() == 1.0f)
            {
                content = "down";
            }
            else if (gamepad.xButton.ReadValue() == 1f)
            {
                content = "up";
            }
            else if (gamepad.leftStick.x.ReadValue() == -1f)
            {
                content = "left";
            }
            else if (gamepad.leftStick.x.ReadValue() == 1f)
            {
                content = "right";
            }
            else if (gamepad.leftStick.y.ReadValue() == 1f)
            {
                content = "forward";
            }
            else if (gamepad.leftStick.y.ReadValue() == -1f)
            {
                content = "backward";
            }

            else if (gamepad.yButton.ReadValue() == 1f)
            {
                content = "Y";
            }
            else if (gamepad.dpad.x.ReadValue() == 1f)
            {
                content = "Dpad x";
            }
            else if (gamepad.dpad.y.ReadValue() == 1f)
            {
                content = "Dpad y";
            }

            else if (gamepad.buttonNorth.ReadValue() == 1.0f)
            {
                content = "North";
            }
            else if (gamepad.buttonWest.ReadValue() == 1.0f)
            {
                content = "West";
            }
            else if (gamepad.buttonSouth.ReadValue() == 1.0f)
            {
                content = "South";
            }
            else if (gamepad.buttonEast.ReadValue() == 1.0f)
            {
                content = "East";
            }

            else if (gamepad.rightStick.x.ReadValue() <= 1f && gamepad.rightStick.x.ReadValue() >= -1f && gamepad.rightStick.x.ReadValue() != 0f
                && gamepad.rightStick.y.ReadValue() <= 1f && gamepad.rightStick.y.ReadValue() >= -1f && gamepad.rightStick.y.ReadValue() != 0f)
            {
                content = "Right Stick: x= " + gamepad.rightStick.x.ReadValue().ToString() + "|y= " + gamepad.rightStick.y.ReadValue().ToString();
            }
            /*else if (gamepad.rightStick.y.ReadValue() <= 1f && gamepad.rightStick.y.ReadValue() >= -1f)
            {
                content = "Right Stick y: " + gamepad.rightStick.y.ReadValue().ToString();
            }*/

            else if (gamepad.circleButton.IsPressed())
            {
                content = "Circle";
            }
            else if (gamepad.leftShoulder.ReadValue() != 0)
            {
                content = "Left Shoulder";
            }
            else if (gamepad.rightShoulder.ReadValue() == 1.0f)
            {
                content = "Right Shoulder";
            }
            else
            {
                content = "";
            }


            /*else if ((gamepad.leftStick.x.ReadValue() <= 1f) & (gamepad.leftStick.x.ReadValue() >= -1f))
            {
               if (gamepad.leftStick.x.ReadValue() < 0.6)
                { content = "X: -1"; }
               if (gamepad.leftStick.x.ReadValue() > 0.6)
                { content = "X: 1"; }
                //content = "X: " + gamepad.leftStick.x.ReadValue().ToString();
            }
            else if ((gamepad.leftStick.y.ReadValue() <= 1f) & (gamepad.leftStick.y.ReadValue() >= -1f))
            {
                if (gamepad.leftStick.y.ReadValue() < 0.6)
                { content = "Y: -1"; }
                if (gamepad.leftStick.y.ReadValue() > 0.6)
                { content = "Y: 1"; }
                //content = "Y: " + gamepad.leftStick.y.ReadValue().ToString();
            }*/





        }
        catch
        {

        }

        if (content != "")
        {
            byte[] value = Encoding.ASCII.GetBytes(content);
            text1.text = content;
            udp.send(value, "192.168.191.101");
        }
    }
}
