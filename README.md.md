A simple and beautiful birthday event for Stardew Valley
-------------------------------------------------------

When this mod is activated (by pressing the B button on the targets computer by default) the following day an email is sent with a birthday text, this day will be sunny. The at 9:00 the event starts in town, at 9:10 Robin tells you that there is an event in town for your birthday. When you go there, the music will changes and there will be fireflights everywhere, you must find 7 fairy roses (each time you grab one butterflies will fly from those) and when you are done, you can speak with Robin in the middle of the town, a special shop will open with special items (1 gold each).

This is a Monodev project, but the code should be usable in any editor.

Working with Stardew Valley 1.4.2 and SMAPI 3.0.1.

![Image](https://raw.githubusercontent.com/pjimenezmateo/birthdayevent-stardew-valley/master/images/Collage.png)

## How to compile

Follow the steps on the [wiki](https://stardewvalleywiki.com/Modding:Modder_Guide/Get_Started#Create_the_project) but use this code.

## Personalization

#### Contents of the letter

Modify BirthdayLetter.cs with the text you prefer

#### Contents of Robin's message

Modify line 290 in ModEntry.cs

#### Position of Robin (and therefore the store)

Modify the line 16 in ModEntry.cs

#### Button that triggers the event

Modify the line 57 in ModEntry.cs

#### Modify the position of the roses


![Image](https://raw.githubusercontent.com/pjimenezmateo/birthdayevent-stardew-valley/master/images/Collage.png)
Modify lines 307 to 313 in ModEntry.cs

#### Modify the items sold at the store

Modify function CreateShop at line 239 in ModEntry.cs, a list of item ids can be found [here.](https://www.ign.com/wikis/stardew-valley/Item_Codes_for_Spawning_Cheat)

#### Modify the times of the event and of Robin message

Modify lines 201 and 211 respectively in ModEntry.cs

## License

This program is licensed under Creative commons Attribution 3.0 Unported, more info : 
http://creativecommons.org/licenses/by/3.0/deed.en_US