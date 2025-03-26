using UnityEngine;
using System.IO;
using System;
using UnityEngine.InputSystem;

namespace PlayerProfiles {

    public struct input {
    public bool up,
                down,
                left,
                right,
                normal,
                special,
                airdash;
    }

    public class botController {
        public input botInputs;
        public input getInput() {
            return botInputs;
        }
    }
    public class controller {
        private bool isController;
        private Gamepad gamepad;
        private UnityEngine.KeyCode[] profile = new UnityEngine.KeyCode[7];
        private bool gamepadAuxWalk;
        public controller(string line) {
            string[] aux = new string[9];
            aux = line.Split(';', 9);
            isController = (aux[8] != "Keyboard");
            if (isController) gamepad = Gamepad.all[0];
            for (int i = 0; i < 7; i++) {
                if (!isController) profile[i] = (UnityEngine.KeyCode)System.Enum.Parse(typeof(UnityEngine.KeyCode), aux[i+1]);
            }
        }

        public input getInput(input prevInput) {
            input playerInput;
            if (!isController) {
                playerInput.up = Input.GetKeyDown(profile[0]) || prevInput.up;
                playerInput.down = Input.GetKey(profile[1]);
                playerInput.left = Input.GetKey(profile[2]);
                playerInput.right = Input.GetKey(profile[3]);
                playerInput.normal = Input.GetKeyDown(profile[4]) || prevInput.normal;
                playerInput.special = Input.GetKeyDown(profile[5]) || prevInput.special;
                playerInput.airdash = Input.GetKeyDown(profile[6]) || prevInput.airdash;
            }
            else {
                Vector2 stick = gamepad.leftStick.ReadValue();
                
                float horizontal = stick.x;
                float vertical = stick.y;
                //Debug.Log(horizontal);
                //Debug.Log(vertical);
                playerInput.up = vertical > 0.5 || prevInput.up;
                playerInput.down = vertical < -0.5;
                playerInput.left = horizontal < -0.2;
                playerInput.right = horizontal > 0.2;
                playerInput.normal = Input.GetKeyDown(KeyCode.JoystickButton1) || prevInput.normal;
                playerInput.special = Input.GetKeyDown(KeyCode.JoystickButton0) || prevInput.special;
                playerInput.airdash = Input.GetKeyDown(KeyCode.JoystickButton4) || prevInput.airdash;
            }
            return playerInput;
        }

        public static input setFalse(input prevInput) {
            input playerInput;
            playerInput.up = false;
            playerInput.down = prevInput.down;
            playerInput.left = prevInput.left;
            playerInput.right = prevInput.right;
            playerInput.airdash = false;
            playerInput.normal = false;
            playerInput.special = false;
            return playerInput;
        }

        public UnityEngine.KeyCode[] getProfile() {
            return this.profile;
        }
        public bool getIsController() {
            return this.isController;
        }
    }
    public class playerProfiles
    {
        private static string profilesFile = Path.Combine(Application.persistentDataPath, "profiles.dat");
        //Orden: Arriba, abajo, izquierda, derecha, salto, escudo, grab, normal, especial

        private static string readProfilesLine(string name) {
        using (var sr = new StreamReader(profilesFile)) {
                string line, resul = "";
                while ((line = sr.ReadLine()) != null) {
                    string[] aux = line.Split(';', 9);
                    if (aux[0] == name) {
                        resul = line;
                        break;
                    }
                }
                return resul;
            }
        }

        private static int getProfilesLine(string name) {
            using (var sr = new StreamReader(profilesFile)) {
                string line;
                int i = 0;
                while ((line = sr.ReadLine()) != null) {
                    char[] separator = {';'};
                    string[] aux = line.Split(separator, 1);
                    if (aux[0] == name) break;
                    i++;
                }
                return i;
            }
        }

        private static void writeLineAtProfilesLine(int n, string text) {
            using (var fs = File.Open(profilesFile, FileMode.Open, FileAccess.ReadWrite))
            {
                int line_number = 0;
                string line;
                var destinationReader = new StreamReader(fs);
                var writer = new StreamWriter(fs);
                while (( line = destinationReader.ReadLine()) != null)
                {
                    writer.WriteLine(line);
                    if (line_number == n)
                    {
                        writer.WriteLine(text);
                    }
                    line_number++;
                }
            }
        }

        public static string getProfile(string name) {
            return readProfilesLine(name);
        }

        public static bool saveProfile(string name, controller prof) {
            string profileLine = name + ";";
            var profile = prof.getProfile();
            for (int i = 0; i < 7; i++) {
                profileLine += profile[i] + ";";
            }
            if (prof.getIsController()) {
                profileLine += "Controller";
            }
            else {
                profileLine += "Keyboard";
            }
            profileLine += Environment.NewLine;
            if (readProfilesLine(name) != "") {
                int n = getProfilesLine(name);
                writeLineAtProfilesLine(n, profileLine);
                return true;
            }
            File.AppendAllText(profilesFile, profileLine);
            return false;
        }
    }
}
