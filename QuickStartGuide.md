# Quick Start Guide #

This is a quick start guide describing the minimum required steps to get the program running.


## Requirements ##

First you need to fill the requirements.

Download and install [.NET Framework 4.0](http://msdn.microsoft.com/en-us/netframework/aa569263.aspx)

## Enable Aero ##

iRTVO uses Windows Presentation Foundation for its graphics. WPF on the other hand relies heavily on Windows' Aero user interface. Without Aero you'll take significant performance hit (in my case fps halved).

[Guide how to enable Aero](http://www.petri.co.il/enable_windows_vista_aero_graphics.htm)

## Setup iRacing.com ##

There are few things that needs to be changed in the sim to get the overlay working properly.

### app.ini ###

Go to My documents/iRacing folder and open **`app.ini`**.

Change the following lines under `Graphics` section:
```
fullScreen=0
reduceFramerateWhenFocusLost=0
serverTransmitMaxCars=64
```

You can change the `fullScreen` setting in the sim from the Options/Graphics uncheking the full screen box.

## iRTVO ##

[Download the latest version](http://code.google.com/p/irtvo/downloads/list) and extract it.

## Ready, get, set, go! ##
  * Launch iRacing
  * Launch iRTVO
  * Start broadcasting!