<h1>Theme Reference</h1>

<h3>Table of Contents</h3>


This document explains all the options that are available for creating custom themes. These settings are stored in `settings.ini`

Items marked in **bold** are mandatory.

# General #

General settings

| **Name** | **Description** | **Example value** | **Note** |
|:---------|:----------------|:------------------|:---------|
| **name** | Name of the theme | Example theme     | Has to be the same as the folder name |
| **author** | Author of the theme | Jari Y            |          |
| **width** | Theme width in pixels | 1280              |          |
| **height** | Theme height in pixels | 720               |          |
| **overlays** | Comma separated list of text overlays | driver,sessionstate,sidepanel | See [ThemeReference#Overlays](ThemeReference#Overlays.md) |
| images   | Comma separated list of images | logo,sidepanel    | See [ThemeReference#Images](ThemeReference#Images.md) |
| tickers  | Comma separated list of tickers | ticker            | See [ThemeReference#Tickers](ThemeReference#Tickers.md) |
| buttons  | Comma separated list of buttons | sidepanel,ticker  | See [ThemeReference#Buttons](ThemeReference#Buttons.md) |
| videos   | Comma separated list of videos | replay            | See [ThemeReference#Videos](ThemeReference#Videos.md) |
| sounds   | Comma separated list of sounds| bleep             | See [ThemeReference#Sounds](ThemeReference#Sounds.md) |
| switchsign | Sign used with intervals (-1.456 or +2 laps) | true              | Default `false` |

# Translation #

Program uses a few natural language words in its text fields. These words can be changed in Translation-section.

| **Name** | **Example value** | **Note** |
|:---------|:------------------|:---------|
|lap       |                   |          |
|laps      |                   |          |
|minutes   |                   |          |
|of        |                   |          |
|remaining |                   |to go     |          |
|race      |                   |          |
|qualify   |                   |          |
|practice  |                   |          |
|out       | retired           |Text shown in user's {interval} when he has quitted|
|gridding  |                   |          |
|pacelap   |                   |          |
|finallap  |                   |          |
|finishing |                   |          |
|invalid   |  -.--             |Text shown in {curlap} and {prevlap} when last lap was invalid (incident or connection issue) |
|replay    | REPLAY            | Text shown in {lapcounter} when the sim is in replay |
|Clear     |                   | Used with {sky} |
|Partly Cloudy|                   | Used with {sky} |
|Mostly Cloudy|                   | Used with {sky} |
|Overcast  |                   | Used with {sky} |

# Overlays #

Each of the overlays defined in General-section must have corresponding `Overlay-overlayname`-section. For example if `overlays=driver,sidepanel` then there must be `[Overlay-driver]` and `[Overlay-sidepanel]`.

| **Name** | **Description** | **Example value** | **Note** |
|:---------|:----------------|:------------------|:---------|
| **width** | Width of the overlay | 540               |          |
| **height** | Heigth of the overlay | 80                |          |
| **left** | Margin from the left border of the window | 64                |          |
| **top**  | Margin from the top border of the window | 582               |          |
| **zIndex** | Stacking order of all items in the theme | 100               | Item with largest zIndex will be shown top most and vice versa |
| **dataset** | The kind of information shown on the text items | followed          | Valid values are `followed`, `standing`, `sessionstate` |
| dataorder | Information used for sorting drivers | fastestlap        | Valid values are `fastestlap`, `previouslap`, `class`, `points`, `position`, `liveposition`. Position is the default. |
| **labels** | Comma sparated list of text labels shown inside of the overlay | num,name,diff     |          |
| fixed    | If set item is shown always | true              |          |
| maxpages | Limits the number of pages | 1                 |          |
| itemsize | Item size of stanging list | 45                | Applies only to overlays that have dataset as `standing` |
| direction | List items to given direction | down              | Applies only to overlays that have dataset as `standing`, valid values are `down`, `up`, `left` and `right`. |
| number   | Number of items to draw | 10                | Applies only to overlays that have dataset as `standing` |
| offset   | Skip a number of drivers from the top | 10                | Applies only to overlays that have dataset as `standing` |
| skip     | Skip a number of drivers from each page | 10                | Applies only to overlays that have dataset as `standing` |
| delay    | Time in seconds before automatically changing page | 5                 | Applies only to overlays that have dataset as `standing` |
| class    | Filters results to be single class and enable multiclass specific items {class**}**| GT                | See [ThemeReference#cars.ini](ThemeReference#cars.ini.md) |


## Overlay labels ##

Each of the labels defined in overlay's labels must be defined in its own section. For example `labels=name,diff` in `[Overlay-driver]` must then have `[Overlay-driver-name]` and `[Overlay-driver-diff]`.

| **Name** | **Description** | **Example value** | **Note** |
|:---------|:----------------|:------------------|:---------|
| **fontsize** | Font size for the text | 34                |          |
| **width** | Object's width in pixels | 400               |          |
| height   | Object's height in pixels | 30                |          |
| text     | Text formatting | {position\_ord}   | See [ThemeReference#text](ThemeReference#text.md) |
| font     | Font used to draw the text | "Arial Black"     | Use the font's full name as seen on Windows Font Viewer (double click the fontfile) |
| fontcolor | Font color for the text | black             | [See all possible colors](http://samples.msdn.microsoft.com/workshop/samples/author/dhtml/colors/ColorTable.htm) |
| fontbold | Font weight for the text | false             | true or false |
| fontitalic | Font style for the text | `true` or `false` |          |
| align    | Text alignment inside its borders | left              | `left`, `right` or `center` |
| left     | Top-left corner's distance from parent object's left border | 0                 |          |
| top      | Top-left corner's distance from parent object's top border | 0                 |          |
| padding-left | Padding from left edge of the label | 2                 |          |
| padding-top | Padding from left edge of the label | 2                 |          |
| padding-right | Padding from left edge of the label | 2                 |          |
| padding-bottom | Padding from left edge of the label | 2                 |          |
| offset   | Add offset to shown driver. | -1                | <sup>2</sup> |
| rounding | How many decimals are shown in time based labels. Only values between 0-3 work. | 3                 |          |
| uppercase | If true the label text is capitilized | true              |          |
| bgcolor  | Background color of the label | #33000000         | [See documentation](http://msdn.microsoft.com/en-us/library/system.windows.media.solidcolorbrush.aspx). |
| background | Filename of the background image | "labelbg.png"     |          |
| dynamic  | Use dynamic filename | true              |          |
| defaultbackground | Filename of the default background image | "labelbg.png"     | Used when dynamic background isn't found. |

<sup>1</sup> Fonts need to be either in the theme folder or installed in Windows' fonts folder.

<sup>2</sup> For example `dataset=followed` and the driver followed is on 3rd place. The using `offset=-1` would show data of the driver on 2nd place and similarly `offset=3` would show data of 5th place driver.

### Text ###

Text property select what information is shown on the overlay. Different datasets have different data available.

#### Dataset: `followed` or `standing` ####

| **Value** | **Description** | **Example output** | **Note** |
|:----------|:----------------|:-------------------|:---------|
| {fullname} | Full name       | Greger Huttu       |          |
| {shortname} | Shorter name    | G Huttu            |          |
| {initials} | Initials        | GHU                |          |
| {driverid} | iRacing customer id | 12345              |          |
| {license} | License         | WC4.99             |          |
| {club}    | Club            | Finland            |          |
| {irating} | iRating         | 3312               |          |
| {car}     | Car             | Radical SR8        | See [ThemeReference#cars.ini](ThemeReference#cars.ini.md) |
| {class}   | Class           | Prototype          | See [ThemeReference#cars.ini](ThemeReference#cars.ini.md) |
| {carnum}  | Car number      | 22                 |          |
| {fastlap} | Fastest lap of the session | 1:23.456           |          |
| {prevlap} | Lap time of previous finished lap | 1:23.456           |          |
| {sector1} | Current lap sector 1 time | 1:23.456           |          |
| {sector2} | Current lap sector 2 time | 1:23.456           |          |
| {sector2} | Current lap sector 3 time | 1:23.456           |          |
| {sector1\_speed\_kph} | Current lap sector 1 top speed in kph | 123.45             |          |
| {sector2\_speed\_kph} | Current lap sector 2 top speed in kph| 123.45             |          |
| {sector2\_speed\_kph} | Current lap sector 3 top speed in kph| 123.45             |          |
| {sector1\_speed\_mph} | Current lap sector 1 top speed in mph| 123.45             |          |
| {sector2\_speed\_mph} | Current lap sector 2 top speed in mph| 123.45             |          |
| {sector2\_speed\_mph} | Current lap sector 3 top speed in mph| 123.45             |          |
| {curlap}  | Lap time  of the current unfinished lap | 1:23.456           |          |
| {lapnum}  | Current lap number | 10                 |          |
| {speedfast\_mph} | Average speed of the fastest lap in miles per hour | 123.45             |          |
| {speedfast\_kph} | Average speed of the fastest lap in kilometers per hour | 123.45             |          |
| {speedprev\_mph} | Average speed of the previous lap in miles per hour | 123.45             |          |
| {speedprev\_kph} | Average speed of the previous lap in kilometers per hour | 123.45             |          |
| {livespeed\_mph} | Current speed in miles per hour | 123                |          |
| {livespeed\_kph} | Current speed in kilometers per hour | 123                |          |
| {position} | Current position at S/F line | 10                 |          |
| {position\_ord} | Current position at S/F line in english ordinal | 3rd                |          |
| {liveposition} | Current real time position | 10                 |          |
| {liveposition\_ord} | Current  real time position in english ordinal | 3rd                |          |
| {positiongain} | Positions gained/lost since the start | +3                 | Works only in race sessions |
| {startposition} | Positions at start | 3                  | Works only in race sessions |
| {startposition\_ord} | Positions at start in english ordinal | 3rd                | Works only in race sessions |
| {highestposition} | Highest position during the race | 3                  | Works only in race sessions |
| {highestposition\_ord} | Highest position during the race in english ordinal | 3rd                | Works only in race sessions |
| {lowestposition} | Lowest position during the race | 29                 | Works only in race sessions |
| {lowestposition\_ord} | Lowest position during the race in english ordinal | 29th               | Works only in race sessions |
| {gap}     | Difference to leader. In practice/qualify difference of fastest laps, in race time/lap difference | +1.234             | Sign can be switched in `[General]` |
| {livegap} | Difference to leader, updated 60 times a second | +1.234             | Sign can be switched in `[General]` |
| {interval} | Time difference to the driver in front | 1.234              |          |
| {liveinterval} | Time difference to the driver in front, updated 60 times a second | 1.234              |          |
| {lapsled} | Number of laps in lead | 22                 | Works only in race sessions |
| {pitstops} | Number of pit stops made | 1                  | Works only in race sessions |
| {pitstoptime} | Last pit stop length or live timer when pitting | 23.56              | Works only in race sessions |
| {classposition} | Current position in class | 1                  | Multiclass variant |
| {classposition\_ord} | Current position (ordinal) in class | 1st                | Multiclass variant |
| {classliveposition} | Current live position in class | 1                  | Multiclass variant |
| {classliveposition\_ord} | Current live position (ordinal) in class | 1st                | Multiclass variant |
| {classpositiongain} | Positions (in class) gained/lost since the start| -1                 | Multiclass variant |
| {classstartposition} | Class position at start | 3                  | Multiclass variant |
| {classstartposition\_ord} | Class position at start in english ordinal | 3rd                | Multiclass variant |
| {classhighestposition} | Highest class position during the race | 3                  | Multiclass variant |
| {classhighestposition\_ord} | Highest class  position during the race in english ordinal | 3rd                | Multiclass variant |
| {classlowestposition} | Lowest class position during the race | 29                 | Multiclass variant |
| {classlowestposition\_ord} | Lowest class position during the race in english ordinal | 29th               | Multiclass variant |
| {classgap} | Difference to class leader | +1.234             | Multiclass variant |
| {classlivegap} | Difference to class leader (realtime) | +1.234             | Multiclass variant |
| {classinterval} | Time difference to same class driver in front | +1.234             | Multiclass variant |
| {classliveinterval} | Time difference to same class driver in front (real time). | +1.234             | Multiclass variant |
| {points}  | Total points    | 12                 | Including points from current finishing position, use with dataorder=points |
| {points\_pos} | Championship position | 12                 | Including points from current finishing position, use with dataorder=points |
| {points\_pos\_ord} | Championship position ordinal | 12th               | Including points from current finishing position, use with dataorder=points |
| {external:#} | Shows data from CSV-file, replace # with column number. | N/A                | See [ThemeReference#External\_Data](ThemeReference#External_Data.md) |

#### Dataset: `sessionstate` ####

| **Value** | **Description** | **Example output** | **Note** |
|:----------|:----------------|:-------------------|:---------|
| {lapstotal} | Total laps in session | 32                 |          |
| {lapscompleted} | Laps completed by the leader | 20                 |          |
| {lapsremaining} | Laps remaining in session | 12                 |          |
| {currentlap} | Lap the leader is currently on | 20                 | This is {lapscompleted} + 1 |
| {timetotal} | Total elapsed time in session | 30:00              |          |
| {timepassed} | Total elapsed time in session | 12:33              |          |
| {timeremaining} | Time remaining in session | 17:27              |          |
| {lapcounter} | Lap counter including transition to "x laps to go" and "final lap" | 12 / 23            | Works just like in the version 1.0 |
| {trackname} | Name of the track | "Infineon long"    | See [ThemeReference#tracks.ini](ThemeReference#tracks.ini.md) |
| {tracklen\_mi} | Length of the track in miles | 1.345              |          |
| {tracklen\_km} | Length of the track in kilometers | 1.345              |          |
| {cautions} | Number of cautions | 5                  | Works only in race sessions with full course cautions |
| {cautionlaps} | Number of laps under caution | 25                 | Works only in race sessions with full course cautions |
| {leadchanges} | Number of lead changes | 5                  |          |
| {sessiontype} | Session type    | race               | See [ThemeReference#Translation](ThemeReference#Translation.md) |
| {turns}   | Number of turns in the track | 12                 |          |
| {city}    | City where the track is located at | Toronto            |          |
| {country} | Country where track is located at | Canada             |          |
| {altitude\_m} | Altitude from sea level in meters | 107                |          |
| {altitude\_ft} | Altitude from sea level in feet | 321                |          |
| {sky}     | Sky condition   | Clear              | See [ThemeReference#Translation](ThemeReference#Translation.md) |
| {tracktemp\_c} | Track temperature in celcius | 32                 |          |
| {tracktemp\_f} | Track temperature in fahrenheit | 107                |          |
| {airtemp\_c} | Air temperature in celcius | 25                 |          |
| {airtemp\_f} | Air temperature in fahrenheit | 107                |          |
| {humidity} | Relative humidity in percent | 88                 |          |
| {fog}     | Relative foggyness in percent | 12                 |          |
| {airpressure\_hg} | Air pressure in mmHg | 760                |          |
| {airpressure\_hpa} | Air pressure hecto pascals | 1013               |          |
| {windspeed\_ms} | Windspeed in meters per second | 8                  |          |
| {windspeed\_kt} | Windspeed in knots | 10                 |          |
| {windspeed\_kph} | Windspeed in kilometers per hour | 5                  |          |
| {winddir\_deg} | Wind direction in degrees | 33                 | Zero degrees facing north, 90 degrees east, etc. |

# Images #

Each of the images defined in General-section must have corresponding `Image-imagename`-section. For example if `images=driver,sidepanel` then there must be `[Image-driver]` and `[Image-sidepanel]`.

| **Value** | **Description** | **Example output** | **Note** |
|:----------|:----------------|:-------------------|:---------|
| **filename** | Filename of the image in theme folder | lights-off.png     | Available image formats are BMP, JPEG, PNG, TIFF, Windows Media Photo, GIF and ICON. |
| **zIndex** | Stacking order of all items in the theme | 10                 | Item with largest zIndex will be shown top most and vice versa |
| width     | Width of the image | 480                | Default is theme width |
| height    | Height of the image | 480                | Default is theme height |
| left      | Margin from the left border of the window | 64                 | Default is 0 |
| top       | Margin from the top border of the window | 582                | Default is 0 |
| dynamic   | Dynamic filename | true               | When true filename will be parsed for driver text properties, see [ThemeReference#Dataset:\_followed\_or\_standing](ThemeReference#Dataset:_followed_or_standing.md) |
| default   | When dynamic filename doesn't exist, use this filename | default.png        | See `dynamic` property |
| fixed     | If set item is shown always | true               |          |

# Tickers #

Each of the tickers defined in General-section must have corresponding `Ticker-tickername`-section. For example if `tickers=standings` then there must be `[Ticker-standings]`.

Tickers have the same properties as overlays. See [ThemeReference#Overlays](ThemeReference#Overlays.md). There are few additional fields:

| **Value** | **Description** | **Example output** | **Note** |
|:----------|:----------------|:-------------------|:---------|
| fillvertical | Sets the direction which labels are stacked. When `true` it creates multi row ticker. `false` is the normal behavior. | false              | Valid values are `true` or `false` |
| fixed     | If set item is shown always | true               |          |
| speed     | Speed of scrolling in 1/60th pixels per second | 6                  | Default is 3 |
| header    | A label to be shown as the first item on the ticker | header             | value "header" references to Ticker-[name](ticker.md)-header |
| footer    | A label to be shown as the last item on the ticker | footer             | similarly as in header |

# Buttons #

Each of the buttons defined in General-section must have corresponding `Button-buttonname`-section. For example if `buttons=sidepanel,ticker` then there must be `[Button-sidepanel]` and `[Button-ticker]`.

Buttons show and hide overlays, images and tickers. Each button can do any combination of these.

| **Value** | **Description** | **Example output** | **Note** |
|:----------|:----------------|:-------------------|:---------|
| text      | Sets the text shown in the button. | Side panel         |          |
| row       | To which row the button is added | 2                  | Starts from 0, which is the default |
| show      | Comma separated list of items that will be visible after button press | Overlay-sidepanel,Image-sidepanel,Trigger-flags | Valid values are any of the overlays, images or tickers defined in `General` or any valid trigger. <sup>1</sup> |
| hide      | Comma separated list of items that will be hidden after button press | Ticker-standings   | See `show` |
| toggle    | Comma separated list of items that will be toggled (visible/hidden) after button press | Oveylay-driver     | See `show` |
| delay     | Delay between automatic page changes | 5                  | Applies only to  buttons which have at least one overlay that has standings-dataset |
| loop      | Loops back to first page after the last one | true               |          |
| hidden    | When true doesn't add button to main window | true               | See hotkey |
| hotkey    | Global hotkey   | Shift-H            | Comprised of modifier and key, possible modifiers are None, Alt, Ctrl, NoRepeat, Shift, Win and possible keys are listed at http://msdn.microsoft.com/en-us/library/system.windows.input.key.aspx |
| replay    | Replays given seconds | 10                 | use 0 to return live |
| playspeed | Changes playspeed | -2                 | negative values are slow motion, positive fast forward and 1 is normal speed |

<sup>1</sup> Special note: When using overlay with dataset `standing` use always `show` to switch to the next page. For example 30 drivers will have 3 pages worth of items if `number` is 10.

# Triggers #

Triggers are much like buttons, but they are triggered automically based on their name. Valid names are:

| **Name** | **Description** |
|:---------|:----------------|
| flagGreen | Green flag      |
| flagYellow | Yellow flag     |
| flagWhite | White flag      |
| flagCheckered | Checkered flag  |
| lightsOff | Triggered few seconds after the start |
| lightsReady | Lights off      |
| lightsSet | Lights on (red) |
| lightsGo | Green light     |
| replay   | Triggered when replay is activated |
| live     | Triggered when coming back to live from replay |

# Videos #

Each of the videos defined in General-section must have corresponding `Video-videoname`-section. For example if `videos=replay` then there must be `[Video-replay]`.

| **Value** | **Description** | **Example output** | **Note** |
|:----------|:----------------|:-------------------|:---------|
| **filename** | Filename of the video in theme folder | replay.wmv         | Windows Media Video is a safe format to start with. |
| **zIndex** | Stacking order of all items in the theme | 10                 | Item with largest zIndex will be shown top most and vice versa |
| loop      | When set the video is looped around and stopped only when "hidden" | true               |          |


# Sounds #

Each of the sounds defined in General-section must have corresponding `Sound-videoname`-section. For example if `sounds=bleep` then there must be `[Sound-bleep]`.

| **Value** | **Description** | **Example output** | **Note** |
|:----------|:----------------|:-------------------|:---------|
| **filename** | Filename of the video in theme folder | beep.wav           | WAVs should always work |
| loop      | When set the sound is looped around and stopped only when "hidden" | true               |          |


# External Data #

External data is stored in a [CSV-file](http://en.wikipedia.org/wiki/Comma-separated_values) called data.csv which is located at the theme folder. This file is semicolon separated table where first column is the driver's customer id.

## Example file ##
```
35698;Finland;Oulu;Team Trellet
12345;USA;Boston;Team USA
```

Now `{external:0}` would become `Finland` or `USA` and `{external:1}` `Team Trellet` or `Team USA` respectively.

If the CSV file doesn't have row for the selected driver the external fields will be ignored.

**Tip:** You can use almost any spreadsheet program to create csv-files, including Google Docs.

# cars.ini #

cars.ini holds the names of cars and classes. File can be in same folder as the irtvo.exe or on theme folder.

Format is `carname="Car name"`. `carname` is the same used in setup folders, but spaces are replaced with backslashes "\". For example:

```
[Cars]
26="Chevrolet Corvette C6R"
39="HPD ARX-01c"
40="Ford GT"

[Multiclass]
26="GT"
40="GT"
39="P2"
```

Cars with same class name (like GT above) are grouped to same class.

# tracks.ini #

tracks.ini holds the names of tracks. File can be in same folder as the irtvo.exe or on theme folder.

Format is `track="Track name"`. `track` is the same used in `camera.ini` in `Documents\iRacing`. For example:

```
[Tracks]
26="Daytona International Speedway Road"
```