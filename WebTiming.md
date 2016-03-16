# Quick start quide #

  * Change the key ($secret) from post.php
  * Upload livetiming folder into your web server
  * Set the URL to post.php as your Post URL in the options window and use the same key
  * If necessary add writing permissions to cache folder (chmod 777)

# Table of Contents #


# Web Timing #

If you have a web server with PHP support you can use web timing to deliver race standings live to your audience. The system works so that iRTVO sends HTTP POST request to the PHP script, which then saves the data into disk. Users then load this data using JavaScript AJAX compile it into a list. iRTVO sends data out with maximum of 1 request per second, the user side updates much slower 5-30 seconds depending on the setup.

# Setup #

## post.php ##

Post.php is script that saves the data from iRTVO into the web server. Authentication is done with simple pre-shared key. Sent data is stored in cache folder where it's read by users.

First lines of post.php hold the configuration parameters.

### secret ###

This is the key you've set on iRTVO options window.

### cachedir ###

This is the directory where json files are stored. This script must have writing access to this directory.

## index.html ##

Index.html is just a regular html-file. It loads livetiming.js which then populates the table on the site. There is also some additional `span`-tags, which will have additional session information. You can even include the timing to your existing site, just load the livetiming.js and add the table.

The table has "zebra striping", so that odd lines are in class "odd". With this you can stylize the table with short bit of CSS.

```
/* even lines */
#standings tbody tr {
  background-color: #fff;
}

/* odd lines */
#standings tbody tr.odd {
  background-color: #f3f3f3;
}
```

Web timing can be configured modifying the `index.html`. At the beginning of the file there's a header `CONFIGURATION` which signifies the start of the configuration parameters.

### Cols ###
This sets which columns are shown in the listing. Possible values are:

| Value | Explanation |
|:------|:------------|
| position | Position    |
| number| Car number  |
| name  | Driver name |
| lap   | Number of laps completed so far |
| lapsled | Number of laps led in a race session |
| fastestlap | Time of the fastest lap in this session |
| previouslap | Time of the previous completed lap in this session |
| interval | Time or lap difference to driver ahead |
| gap   | Time difference to leader |
| pit   | Number of pitstops |
| sector1 | Sector 1 time |
| sector2 | Sector 2 time |
| sector3 | Sector 3 time |

### colNames ###
This sets the headers for the columns set on `Cols`. Number of items should be the same as in `Cols`.

### updateFreq ###
This is the frequency new updates are checked in seconds. If you expect large crowds use big values like 20-40. For smaller amount of users you can go as low as 2 but no lower than the update frequency in you iRTVO settings.

### cachedir ###
This sets the directory where json files are looked for. Use the same values as in post.php