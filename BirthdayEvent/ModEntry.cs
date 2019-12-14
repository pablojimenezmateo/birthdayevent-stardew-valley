using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace BirthdayEvent
{
    public class ModEntry : Mod
    {
        IModHelper helper;

        // The position the NPC with the shop will have
        Vector2 npcPosition = new Vector2(28, 67);

        // The NPC
        StardewValley.NPC newNpc = null;

        // A dictionary which contains all the items
        Dictionary<ISalable, int[]> items = new Dictionary<ISalable, int[]>();

        // The actual shop
        StardewValley.Menus.ShopMenu shop;

        // Wether the birthday has already been started
        Boolean alreadyStarted = false;

        // If the B button has been pressed
        Boolean bPressed = false;

        // Used to spawn fireflies every 30 minutes
        int every30Minutes = 0;

        // The positions of the fairy roses
        Vector2[] rosesPositions = new Vector2[7];
        bool[] rosesFound = new bool[7];
        bool allRosesFound = false;

        //Entry point of the mod
        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            this.helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        //Used to start the birthday
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            // Ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // If the B button is pressed
            if (!bPressed && e.Button == SButton.B)
            {
                this.Monitor.Log("Tomorrow birthday :D", LogLevel.Info);

                bPressed = true;

                // In case they already have the birthday email, remove it
                if (Game1.player.mailReceived.Contains("Birthday"))
                {
                    Game1.player.mailReceived.Remove("Birthday");
                }

                //Create the letter
                Helper.Content.AssetEditors.Add(new BirthdayLetter());

                //Send the letter for tomorrow
                Game1.addMailForTomorrow("Birthday");

                //This will start the birthday in the next morning
                this.helper.Events.GameLoop.DayStarted += this.OnDayStarted;

                //Set the weather for tomorrow to sunny :D
                Game1.weatherForTomorrow = 4;
            }

            // On mouse click
            if (alreadyStarted && Game1.player.currentLocation.Name.Equals("Town") && (e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight))
            {

                ICursorPosition cursorPos = this.Helper.Input.GetCursorPosition();
                Vector2 pos = cursorPos.GrabTile;

                // Check for shop
                // This is called when right clicking on Robin
                if (pos == npcPosition && e.Button == SButton.MouseRight)
                {
                    if (allRosesFound)
                    {
                        this.Monitor.Log("Click on Robin", LogLevel.Info);

                        // Activate shop
                        shop = new StardewValley.Menus.ShopMenu(items, 0, null);

                        Game1.activeClickableMenu = shop;

                        //https://github.com/AdamMcIntosh/StawdewValley/blob/master/Menus/ShopMenu.cs
                    }
                    else
                    {
                        // Send a message
                        string message = "You have to find all the roses before you can talk to Robin!";
                        Game1.activeClickableMenu = new StardewValley.Menus.DialogueBox(message);
                    }
                }

                // Check to pick roses
                if (!allRosesFound)
                {
                    // Check if a rose has been found
                    for (int i = 0; i < 7; i++)
                    {
                        if (pos == rosesPositions[i])
                        {
                            rosesFound[i] = true;
                            this.Monitor.Log("Rose picked", LogLevel.Info);

                            // Spawn some butterflies
                            Random rnd = new Random();

                            for (int j = 0; j < 10; j++)
                            {
                                Butterfly b = new Butterfly(new Vector2((int)pos.X, (int)pos.Y));

                                Game1.getLocationFromName("Town").addCritter(b);
                            }
                        }
                    }

                    // Set to true when all the roses have been found
                    allRosesFound = CheckAllRosesPicked();

                    // Send a message
                    if (allRosesFound)
                    {
                        string message = "You have found all the roses! ^Go talk to Robin!";
                        Game1.activeClickableMenu = new StardewValley.Menus.DialogueBox(message);
                    }
                }
            }
        }

        // Called when the day ends
        private void CleanupBirthday(object sender, DayEndingEventArgs e)
        {
            // Delete the merchant
            Game1.removeThisCharacterFromAllLocations(newNpc);

            // Unhook the clean call
            this.helper.Events.GameLoop.DayEnding -= this.CleanupBirthday;

            // Unhook the button event
            this.helper.Events.Input.ButtonPressed -= this.OnButtonPressed;

            //Unhook the timed event
            this.helper.Events.GameLoop.TimeChanged -= this.OnTimeChanged;
        }

        // Check if all the roses have been found
        private bool CheckAllRosesPicked()
        {
            for (int i = 0; i < 7; i++)
            {
                if (rosesFound[i] == false)
                {
                    return false;
                }
            }

            return true;
        }

        //The day of the birthday
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // The actual day of the birthday
            this.Monitor.Log("Enabled", LogLevel.Info);

            // Hook the on time changed event to our function
            this.helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;

            // Create the items for the shop
            this.CreateShop();

            // Unhook the day started call
            this.helper.Events.GameLoop.DayStarted -= this.OnDayStarted;

            // Hook cleanup
            this.helper.Events.GameLoop.DayEnding += this.CleanupBirthday;
        }

        //Only active on the birthday day, send a message at 9:00
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // Message at 9:00
            if (e.NewTime == 900)
            {
                //Send a message to all
                Game1.addHUDMessage(new HUDMessage("Birthday celebration has started in town!", 1));

                // Flag to check if the birthday has been started
                this.alreadyStarted = true;
            }

            // Birthday at 9:10
            if (e.NewTime == 910)
            {
                this.StartBirthdayEvent();
            }

            // Add fireflies around the NPC every 30 minutes
            if (alreadyStarted && every30Minutes % 3 == 0)
            {
                Random rnd = new Random();

                for (int i = 0; i < 50; i++)
                {
                    Firefly f = new Firefly(new Vector2(rnd.Next((int)(npcPosition.X - 15), (int)(npcPosition.X + 15)), rnd.Next((int)(npcPosition.Y - 15), (int)(npcPosition.Y + 15))));

                    Game1.getLocationFromName("Town").addCritter(f);
                }
            }

            every30Minutes += 1;

            //Debug
            GameLocation location = Game1.currentLocation;
            int playerX = (int)Math.Floor(Game1.player.Position.X / Game1.tileSize);
            int playerY = (int)Math.Floor(Game1.player.Position.Y / Game1.tileSize);
            this.Monitor.Log($"Player at ({playerX}, {playerY}) name {location.Name}", LogLevel.Info);
        }

        // Adds the items to the shop
        private void CreateShop()
        {
            StardewValley.Object itm;
            int[] q = new int[2];
            q[0] = 1; //Price 
            q[1] = 1; //Quantity

            // Add all the tree saplings
            for (int i = 0; i < 6; i++)
            {
                itm = new StardewValley.Object(628 + i, 1, false, 0, 4);
                items.Add(itm, q);
            }

            // Add hay
            itm = new StardewValley.Object(178, 1, false, 0, 4);
            items.Add(itm, new int[2] { 1, 1998 });
        }

        private void StartBirthdayEvent()
        {
            //Here is were I am supposed to do all the cool stuff, is birthday day and we are in town
            this.Monitor.Log("Event started!", LogLevel.Info);

            StardewValley.NPC protoNPC = null;

            //Spawn a custom merchant with Robin's sprite
            foreach (StardewValley.GameLocation loc in StardewValley.Game1.locations)
            {
                foreach (StardewValley.NPC npc in loc.characters)
                {
                    if (npc.getName() == "Robin")
                    {
                        protoNPC = npc;
                    }
                }
            }

            newNpc = new StardewValley.NPC(protoNPC.Sprite, npcPosition, "Town", 2, "Robin", new Dictionary<int, int[]>(), protoNPC.Portrait, false);
            newNpc.setTileLocation(npcPosition);
            newNpc.Speed = protoNPC.Speed;

            // Spawn Robin in town
            Game1.getLocationFromName("Town").addCharacter(newNpc);

            // Try to stop Robin
            newNpc.Halt();
            newNpc.stopWithoutChangingFrame();
            newNpc.movementPause = 1000000000;

            //Opens a dialogue as soon as the village is entered
            newNpc.setNewDialogue("Hello!, someone special has prepared a minigame for you! Find the 7 fairy roses hidden in town and come talk to me in the middle of the plaza!");

            // Show dialogue
            Game1.drawDialogue(newNpc);

            // Change music, if we are currently on town change directly, else set
            // up a hook to change the music when the player enters town
            if (Game1.player.currentLocation.Name.Equals("Town"))
            {
                Game1.changeMusicTrack("WizardSong");
            }
            else
            {
                helper.Events.Player.Warped += OnPlayerWarped;
            }

            // Roses positions
            rosesPositions[0] = new Vector2(15, 52);
            rosesPositions[1] = new Vector2(16, 52);
            rosesPositions[2] = new Vector2(15, 72);
            rosesPositions[3] = new Vector2(60, 55);
            rosesPositions[4] = new Vector2(44, 86);
            rosesPositions[5] = new Vector2(61, 97);
            rosesPositions[6] = new Vector2(62, 97);

            // Set the vector to check if they have been picked up and spawn them
            for (int i = 0; i < 7; i++)
            {
                rosesFound[i] = false;

                // Spawn the roses
                Game1.getLocationFromName("Town").dropObject(new StardewValley.Object(595, 1, false, -1, 4), rosesPositions[i] * 64f, Game1.viewport, true, (Farmer)null);
            }
        }

        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name.Equals("Town"))
            {
                Game1.changeMusicTrack("WizardSong");

                // Unhook itself
                helper.Events.Player.Warped -= OnPlayerWarped;
            }
        }
    }
}

// Event: https://stardewvalleywiki.com/Modding:Event_data
// Hookable events: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Events#Events


//Maps: https://stardewvalleywiki.com/Modding:Maps

//Multiplayer something: https://github.com/Pathoschild/smapi-mod-dump/blob/master/source/%7Ejanavarro95/GeneralMods/HappyBirthday/Framework/MultiplayerSupport.cs
//Game methods: https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI/Framework/SGame.cs
//Hats: https://github.com/MouseyPounds/stardew-mods/blob/master/Festival%20of%20the%20Mundane/Source/ShadowFestival/ShadowFestival/ModEntry.cs
//NPC: https://community.playstarbound.com/threads/smapi-stardew-modding-api.108375/page-23
//Custom NPC: https://github.com/janavarro95/Stardew_Valley_Mods/blob/master/GeneralMods/CustomNPCFramework/Class1.cs

//Freeze time: https://github.com/janavarro95/Stardew_Valley_Mods/blob/master/GeneralMods/TimeFreeze/TimeFreeze.cs