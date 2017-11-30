﻿using System;
using System.Diagnostics;
using System.Drawing;
using L2RBot.Common.Enum;
using System.Threading;

namespace L2RBot
{
    /// <summary>
    /// Used to perform and complete the Lineage 2 Revolution Main Quest.
    /// </summary>
    class Main : Quest
    {

        

        //Pixel objects
        private Pixel mainQuest; //used for mainQuest clicking

        private Pixel movePixel;//used in conjunction with timer to detect non-movement.

        private Pixel[] blueArrow = new Pixel[2]; //the pixels the blue arrow graphic detection

        private Pixel[] questDone = new Pixel[2]; //the pixel value for the quest done graphic detection

        private Pixel[] questBubble = new Pixel[2]; //the pixel value for the quest chat bubble

        /// <summary>
        /// Constructs MainQuest data objects.
        /// </summary>
        /// <param name="APP">Android Emulator Process object</param>
        public Main(Process APP) : base(APP)
        {
            IdleTimeInMs = 30000;

            Helper = new QuestHelper(App)
            {
                Quest = QuestType.Main,
                Deathcount = this.Deathcount,
                Respawn = this.Respawn
            };

            //Pixel objects
            //used to click main quest
            mainQuest = new Pixel
            {
                Color = Color.FromArgb(255, 255, 255, 255),
                Point = new Point(125, 250)
            };

            //used in conjunction with timer to click main quest if no movement has been detected
            movePixel = new Pixel
            {
                Color = L2RBot.Screen.GetColor(Screen, 230, 490),
                Point = new Point(230, 490)
            };

            //the blue arrow that is present during early game main quests, lots of bugs in early quests
            blueArrow[0] = new Pixel
            {
                Color = Color.FromArgb(255, 54, 103, 123),
                Point = new Point(282, 243)
            };
            blueArrow[1] = new Pixel
            {
                Color = Color.FromArgb(255, 46, 91, 108),
                Point = new Point(282, 249)
            };

            //Done graphic that is present upon main quest completion.
            questDone[0] = new Pixel
            {
                Color = Color.White,
                Point = new Point(218, 260)//White D in 'Done' graphic.
            };

            questDone[1] = new Pixel
            {
                Color = Color.White,
                Point = new Point(220, 260)//Center of white D in 'Done' graphic.
            };

            //Bubble that appears over an NPC when they need a talkin to.
            questBubble[0] = new Pixel
            {
                Color = Color.FromArgb(255, 226, 226, 226),
                Point = new Point(692, 249)
            };
            questBubble[1] = new Pixel
            {
                Color = Color.FromArgb(255, 63, 63, 63),
                Point = new Point(679, 262)
            };
        }


        /// <summary>
        /// Starts quest.
        /// </summary>
        public void Start()
        {
            UpdateScreen();

            User32.SetForegroundWindow(App.MainWindowHandle);

            Sleep();//Sleep before to prevent actions from occuring to early.

            //InitClick();

            QuestDone();

            QuestBubble();

            //IdleCheck();

            Helper.Start();

            IsHelperComplete();

            Sleep();//Sleep after to prevent actions from occuring on the next active window.
        }

        /// <summary>
        /// Detects the Blue Arrow animation that overlays the MainQuest at lower levels.
        /// </summary>
        private void BlueArrowPresent()
        {
            if (blueArrow[0].IsPresent(Screen, 5) & blueArrow[1].IsPresent(Screen, 5))
            {
                Click(mainQuest.Point);
            }
        }

        /// <summary>
        /// Detects movement using Click(), calls Click if idle is detected.
        /// </summary>
        private void IdleCheck()
        {
            if (Timer.ElapsedMilliseconds > IdleTimeInMs)//Checks both click timers.
            {
                if (movePixel.IsPresent(Screen, 2))//Looks to see if your map has moved.
                {
                    movePixel.UpdateColor(Screen);
                    Click(mainQuest.Point);
                }
                movePixel.UpdateColor(Screen);
                Timer.Stop();
                Timer.Reset();
                Timer.Start();
            }
        }

        /// <summary>
        /// detects the QuestBubble containing an elipsis(...), calls Click() upon detection
        /// </summary>
        private void QuestBubble()
        {
            if (questBubble[0].IsPresent(Screen, 2) & questBubble[1].IsPresent(Screen, 2) && Timer.ElapsedMilliseconds > IdleTimeInMs)
            {
                Click(mainQuest.Point);
            }
            //Bot.PopUpKiller(App);
        }

        /// <summary>
        /// Performs the initial Click() upon starting the quest using the initialClick bool, setting true upon initial click
        /// </summary>
        private void InitClick()
        {
            if (InitialClick.Equals(false))
            {
                Click(mainQuest.Point);
                InitialClick = true;
            }
        }

        /// <summary>
        /// Detected the Quest Done graphic that overlays the main quest upon completion, calls Click() of mainQuest.Point
        /// </summary>
        private void QuestDone()
        {
            if (questDone[0].IsPresent(Screen, 2) && !questDone[1].IsPresent(Screen, 2))
            {
                Click(mainQuest.Point);
            }
        }
    }
}