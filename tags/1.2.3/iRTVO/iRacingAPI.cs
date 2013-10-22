﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iRSDKSharp;

using System.Threading;
using System.Text.RegularExpressions;
using System.Globalization;

using System.IO;
using Ini;

namespace iRTVO
{
    class iRacingAPI
    {
        public iRacingSDK sdk;
        private int lastUpdate;

        public iRacingAPI()
        {
            sdk = new iRacingSDK();
            lastUpdate = -1;
        }

        private string parseStringValue(string yaml, string key, string suffix = "")
        {
            int length = yaml.Length;
            int start = yaml.IndexOf(key, 0, length);
            if (start >= 0)
            {
                int end = yaml.IndexOf("\n", start, length - start);
                if (end < 0)
                    end = yaml.Length;
                start += key.Length + 2; // skip key name and ": " -separator
                if (start < end)
                    return yaml.Substring(start, end - start - suffix.Length).Trim();
                else
                    return null;
            }
            else
                return null;
        }

        private int parseIntValue(string yaml, string key, string suffix = "")
        {
            string parsedString = parseStringValue(yaml, key, suffix);
            int value;
            bool result = Int32.TryParse(parsedString, out value);
            if (result)
                return value;
            else
                return Int32.MaxValue;
        }

        private float parseFloatValue(string yaml, string key, string suffix = "")
        {
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

            string parsedString = parseStringValue(yaml, key, suffix);
            double value;
            bool result = Double.TryParse(parsedString, style, culture, out value);
            if (result)
                return (float)value;
            else
                return float.MaxValue;
        }

        private Double parseDoubleValue(string yaml, string key, string suffix = "")
        {
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

            string parsedString = parseStringValue(yaml, key, suffix);
            double value;
            bool result = Double.TryParse(parsedString, style, culture, out value);
            if (result)
                return value;
            else
                return Double.MaxValue;
        }

        private static Dictionary<String, Sessions.SessionInfo.sessionType> sessionTypeMap = new Dictionary<String, Sessions.SessionInfo.sessionType>()
        {
            {"Practice", Sessions.SessionInfo.sessionType.practice},
            {"Open Qualify", Sessions.SessionInfo.sessionType.qualify},
            {"Lone Qualify", Sessions.SessionInfo.sessionType.qualify},
            {"Race", Sessions.SessionInfo.sessionType.race}
        };

        private void parseFlag(Sessions.SessionInfo session, Int64 flag)
        {
            /*
            Dictionary<Int32, sessionFlag> flagMap = new Dictionary<Int32, sessionFlag>()
            {
                // global flags
                0x00000001 = sessionFlag.checkered,
                0x00000002 = sessionFlag.white,
                green = sessionFlag.green,
                yellow = 0x00000008,
             * 
                red = 0x00000010,
                blue = 0x00000020,
                debris = 0x00000040,
                crossed = 0x00000080,
             * 
                yellowWaving = 0x00000100,
                oneLapToGreen = 0x00000200,
                greenHeld = 0x00000400,
                tenToGo = 0x00000800,
             * 
                fiveToGo = 0x00001000,
                randomWaving = 0x00002000,
                caution = 0x00004000,
                cautionWaving = 0x00008000,

                // drivers black flags
                black = 0x00010000,
                disqualify = 0x00020000,
                servicible = 0x00040000, // car is allowed service (not a flag)
                furled = 0x00080000,
             * 
                repair = 0x00100000,

                // start lights
                startHidden = 0x10000000,
                startReady = 0x20000000,
                startSet = 0x40000000,
                startGo = 0x80000000,

            };*/

            //Console.WriteLine(flag.ToString("X"));
            /*
            Console.WriteLine("flag: "+ (flag & 0x0000000f).ToString("X"));
            Console.WriteLine("special flag: " + ((flag & 0x000000f0) >> 4 * 1).ToString("X"));
            Console.WriteLine("special flag: " + ((flag & 0x00000f00) >> 4 * 2).ToString("X"));
            Console.WriteLine("special flag: " + ((flag & 0x0000f000) >> 4 * 3).ToString("X"));
            Console.WriteLine("driver flag: "  + ((flag & 0x000f0000) >> 4 * 4).ToString("X"));
            Console.WriteLine("driver flag: "  + ((flag & 0x00f00000) >> 4 * 5).ToString("X"));
            Console.WriteLine("start lights: " + ((flag & 0xf0000000) >> 4 * 7).ToString("X"));
            */
            Int64 regularFlag = flag & 0x0000000f;
            Int64 specialFlag = (flag & 0x0000f000) >> (4 * 3);
            Int64 startlight = (flag & 0xf0000000) >> (4 * 7);

            if (regularFlag == 0x8 || specialFlag >= 0x4)
                session.Flag = Sessions.SessionInfo.sessionFlag.yellow;
            else if (regularFlag == 0x2)
                session.Flag = Sessions.SessionInfo.sessionFlag.white;
            else if (regularFlag == 0x1)
                session.Flag = Sessions.SessionInfo.sessionFlag.checkered;
            else
                session.Flag = Sessions.SessionInfo.sessionFlag.green;
            

            if (startlight == 0x1)
                session.StartLight = Sessions.SessionInfo.sessionStartLight.off;
            else if (startlight == 0x2)
                session.StartLight = Sessions.SessionInfo.sessionStartLight.ready;
            else if (startlight == 0x4)
                session.StartLight = Sessions.SessionInfo.sessionStartLight.set;
            else if (startlight == 0x8)
                session.StartLight = Sessions.SessionInfo.sessionStartLight.go;
        }

        private void parser(string yaml) {
            int start = 0;
            int end = 0;
            int length = 0;

            length = yaml.Length;
            start = yaml.IndexOf("DriverInfo:\n", 0, length);
            end = yaml.IndexOf("\n\n", start, length - start);

            string DriverInfo = yaml.Substring(start, end - start);

            length = DriverInfo.Length;
            start = DriverInfo.IndexOf(" Drivers:\n", 0, length);
            end = length;

            string Drivers = DriverInfo.Substring(start, end - start - 1);
            string[] driverList = Drivers.Split(new string[] { "\n - " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string driver in driverList)
            {
                int userId = parseIntValue(driver, "UserID");
                if (userId < Int32.MaxValue)
                {
                    int index = SharedData.Drivers.FindIndex(d => d.UserId.Equals(userId));
                    if (index < 0 && parseStringValue(driver, "CarPath") != "safety pcfr500s")
                    {
                        DriverInfo driverItem = new DriverInfo();
                        driverItem.Name = parseStringValue(driver, "UserName");
                        driverItem.Shortname = parseStringValue(driver, "AbbrevName");
                        driverItem.Initials = parseStringValue(driver, "Initials");
                        driverItem.Club = parseStringValue(driver, "ClubName");
                        driverItem.NumberPlate = parseStringValue(driver, "CarNumber");
                        //driverItem.Car = parseStringValue(driver, "CarPath");
                        driverItem.CarId = parseIntValue(driver, "CarID");
                        driverItem.CarClass = parseIntValue(driver, "CarClassID");
                        driverItem.UserId = parseIntValue(driver, "UserID");
                        driverItem.CarIdx = parseIntValue(driver, "CarIdx");

                        int liclevel = parseIntValue(driver, "LicLevel");
                        int licsublevel = parseIntValue(driver, "LicSubLevel");

                        switch (liclevel)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                driverItem.SR = "R" + ((double)licsublevel / 100).ToString("0.00");
                                break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                driverItem.SR = "D" + ((double)licsublevel / 100).ToString("0.00");
                                break;
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                                driverItem.SR = "C" + ((double)licsublevel / 100).ToString("0.00");
                                break;
                            case 14:
                            case 15:
                            case 16:
                            case 17:
                                driverItem.SR = "B" + ((double)licsublevel / 100).ToString("0.00");
                                break;
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                                driverItem.SR = "A" + ((double)licsublevel / 100).ToString("0.00");
                                break;
                            case 22:
                            case 23:
                            case 24:
                            case 25:
                                driverItem.SR = "P" + ((double)licsublevel / 100).ToString("0.00");
                                break;
                            case 26:
                            case 27:
                            case 28:
                            case 29:
                                driverItem.SR = "WC" + ((double)licsublevel / 100).ToString("0.00");
                                break;
                            default:
                                driverItem.SR = "Unknown";
                                break;
                        }

                        SharedData.Drivers.Add(driverItem);

                        driverItem.CarClass = -1;
                        int carclass = parseIntValue(driver, "CarClassID");
                        int freeslot = -1;

                        for(int i=0; i < SharedData.Classes.Length; i++) {
                            if (SharedData.Classes[i] == carclass)
                            {
                                driverItem.CarClass = i;
                            }
                            else if(SharedData.Classes[i] == -1 && freeslot < 0) {
                                freeslot = i;
                            }
                        }

                        if (driverItem.CarClass < 0 && freeslot >= 0)
                        {
                            SharedData.Classes[freeslot] = carclass;
                            driverItem.CarClass = freeslot;
                        }
                    }
                }
            }

            length = yaml.Length;
            start = yaml.IndexOf("SessionInfo:\n", 0, length);
            end = yaml.IndexOf("\n\n", start, length - start);

            string SessionInfo = yaml.Substring(start, end - start);

            length = SessionInfo.Length;
            start = SessionInfo.IndexOf(" Sessions:\n", 0, length);
            end = length;

            string Sessions = SessionInfo.Substring(start, end - start);
            string[] sessionList = Sessions.Split(new string[] { "\n - " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string session in sessionList)
            {
                int sessionNum = parseIntValue(session, "SessionNum");
                if (sessionNum < Int32.MaxValue)
                {
                    int sessionIndex = SharedData.Sessions.SessionList.FindIndex(s => s.Id.Equals(sessionNum));
                    if (sessionIndex < 0) // add new session item
                    {
                        Sessions.SessionInfo sessionItem = new Sessions.SessionInfo();
                        sessionItem.Id = sessionNum; 
                        sessionItem.LapsTotal = parseIntValue(session, "SessionLaps");
                        sessionItem.SessionLength = parseFloatValue(session, "SessionTime", "sec");
                        sessionItem.Type = sessionTypeMap[parseStringValue(session, "SessionType")];

                        if (sessionItem.Type == iRTVO.Sessions.SessionInfo.sessionType.race)
                        {
                            sessionItem.FinishLine = parseIntValue(session, "SessionLaps") + 1;
                        }
                        else
                        {
                            sessionItem.FinishLine = Int32.MaxValue;
                        }

                        if (sessionItem.FinishLine < 0)
                        {
                            sessionItem.FinishLine = Int32.MaxValue;
                        }

                        if (SharedData.isLive)
                        {
                            sessionItem.Cautions = parseIntValue(session, "ResultsNumCautionFlags");
                            sessionItem.CautionLaps = parseIntValue(session, "ResultsNumCautionLaps");
                            sessionItem.LeadChanges = parseIntValue(session, "ResultsNumLeadChanges");
                            sessionItem.LapsComplete = parseIntValue(session, "ResultsLapsComplete");

                            length = session.Length;
                            start = session.IndexOf("   ResultsFastestLap:\n", 0, length);
                            end = length;
                            string ResultsFastestLap = session.Substring(start, end - start);

                            sessionItem.FastestLap = parseFloatValue(ResultsFastestLap, "FastestTime");
                            int index = SharedData.Drivers.FindIndex(d => d.CarIdx.Equals(parseIntValue(ResultsFastestLap, "CarIdx")));
                            if (index >= 0)
                            {
                                sessionItem.FastestLapDriver = SharedData.Drivers[index];
                                sessionItem.FastestLapNum = parseIntValue(ResultsFastestLap, "FastestLap");
                            }
                        }
                        SharedData.Sessions.SessionList.Add(sessionItem);
                        sessionIndex = SharedData.Sessions.SessionList.FindIndex(s => s.Id.Equals(sessionNum));
                    }
                    else // update only non fixed fields
                    {
                        if (SharedData.isLive)
                        {
                            //SharedData.Sessions.SessionList[sessionIndex].Cautions = parseIntValue(session, "ResultsNumCautionFlags");
                            //SharedData.Sessions.SessionList[sessionIndex].CautionLaps = parseIntValue(session, "ResultsNumCautionLaps");
                            SharedData.Sessions.SessionList[sessionIndex].LeadChanges = parseIntValue(session, "ResultsNumLeadChanges");
                            SharedData.Sessions.SessionList[sessionIndex].LapsComplete = parseIntValue(session, "ResultsLapsComplete");

                            length = session.Length;
                            start = session.IndexOf("   ResultsFastestLap:\n", 0, length) + "   ResultsFastestLap:\n".Length;
                            end = length;
                            string ResultsFastestLap = session.Substring(start, end - start);

                            SharedData.Sessions.SessionList[sessionIndex].FastestLap = parseFloatValue(ResultsFastestLap, "FastestTime");
                            int index = SharedData.Drivers.FindIndex(d => d.CarIdx.Equals(parseIntValue(ResultsFastestLap, "CarIdx")));
                            if (index >= 0)
                            {
                                SharedData.Sessions.SessionList[sessionIndex].FastestLapDriver = SharedData.Drivers[index];
                                SharedData.Sessions.SessionList[sessionIndex].FastestLapNum = parseIntValue(ResultsFastestLap, "FastestLap");
                            }
                        }
                        else
                        {
                            SharedData.Sessions.SessionList[sessionIndex].LapsComplete = SharedData.Sessions.SessionList[sessionIndex].getLeader().CurrentLap.LapNum;
                        }
                    }

                    length = session.Length;
                    start = session.IndexOf("   ResultsPositions:\n", 0, length);
                    end = session.IndexOf("   ResultsFastestLap:\n", start, length - start);

                    string Standings = session.Substring(start, end - start);
                    string[] standingList = Standings.Split(new string[] { "\n   - " }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string standing in standingList)
                    {
                        int carIdx = parseIntValue(standing, "CarIdx");
                        if (carIdx < Int32.MaxValue)
                        {
                            Sessions.SessionInfo.StandingsItem standingItem = new Sessions.SessionInfo.StandingsItem();
                            standingItem = SharedData.Sessions.SessionList[sessionIndex].FindDriver(carIdx);

                            if (SharedData.isLive)
                            {
                                if (parseFloatValue(standing, "LastTime") > 0)
                                {
                                    if (parseFloatValue(standing, "LastTime") < SharedData.Sessions.SessionList[sessionIndex].FastestLap && SharedData.Sessions.SessionList[sessionIndex].FastestLap > 0)
                                    {
                                        Event ev = new Event(
                                            Event.eventType.fastlap,
                                            SharedData.Sessions.SessionList[sessionIndex].CurrentReplayPosition,
                                            standingItem.Driver,
                                            "New session fastest lap (" + iRTVO.Overlay.floatTime2String(parseFloatValue(standing, "LastTime"), 3, false) + ")",
                                            SharedData.Sessions.SessionList[sessionIndex].Type,
                                            parseIntValue(standing, "LapsComplete")
                                        );
                                        SharedData.Events.List.Add(ev);
                                    }
                                    /*
                                    else if (parseFloatValue(standing, "LastTime") < parseFloatValue(standing, "FastestTime") && standingItem.FastestLap > 0)
                                    {
                                        Event ev = new Event(
                                            Event.eventType.fastlap,
                                            SharedData.Sessions.CurrentSession.CurrentReplayPosition,
                                            standingItem.Driver,
                                            "New PB (" + iRTVO.Overlay.floatTime2String(parseFloatValue(standing, "LastTime"), 3, false) + ")",
                                            SharedData.Sessions.SessionList[sessionIndex].Type,
                                            parseIntValue(standing, "LapsComplete")
                                        );
                                        SharedData.Events.List.Add(ev);
                                    }
                                         * */
                                }

                                if (parseFloatValue(standing, "FastestTime") < SharedData.Sessions.SessionList[sessionIndex].FastestLap ||
                                    SharedData.Sessions.SessionList[sessionIndex].FastestLap <= 0)
                                {
                                    SharedData.Sessions.SessionList[sessionIndex].FastestLap = parseFloatValue(standing, "FastestTime");
                                }

                                //LapInfo newLap = new LapInfo();
                            
                                //newLap.LapNum = parseIntValue(standing, "LapsComplete");
                                if (standingItem.Finished == false)
                                {
                                    /*
                                    if (standingItem.PreviousLap.LapTime != parseFloatValue(standing, "LastTime") && (parseFloatValue(standing, "LastTime") - standingItem.PreviousLap.LapTime) < 10)
                                    {
                                        //Console.WriteLine(standingItem.Driver.Name + " official laptime " + parseFloatValue(standing, "LastTime") + ", lap: " + standingItem.PreviousLap.LapNum);
                                        //Console.WriteLine((parseFloatValue(standing, "LastTime") - standingItem.PreviousLap.LapTime));
                                    }
                                    */

                                    standingItem.PreviousLap.LapTime = parseFloatValue(standing, "LastTime");

                                    if (standingItem.PreviousLap.LapTime <= 1)
                                    {
                                        standingItem.PreviousLap.LapTime = standingItem.CurrentLap.LapTime;
                                    }
                                    standingItem.PreviousLap.Position = parseIntValue(standing, "Position");
                                    standingItem.PreviousLap.Gap = parseFloatValue(standing, "Time");
                                    standingItem.PreviousLap.GapLaps = parseIntValue(standing, "Lap");
                                }
                                else
                                {
                                    standingItem.CurrentLap.Position = parseIntValue(standing, "Position");
                                    standingItem.CurrentLap.Gap = parseFloatValue(standing, "Time");
                                    standingItem.CurrentLap.GapLaps = parseIntValue(standing, "Lap");
                                    /*
                                    if (standingItem.Position > 1 && standingItem.CurrentLap.Gap == 0)
                                    {
                                        standingItem.CurrentLap.Gap = standingItem.PreviousLap.Gap;
                                        standingItem.CurrentLap.GapLaps = standingItem.PreviousLap.GapLaps;
                                    }
                                     * */
                                }
                            }

                            if(standingItem.Driver.CarIdx < 0)
                            {
                                // insert item
                                int driverIndex = SharedData.Drivers.FindIndex(d => d.CarIdx.Equals(carIdx));
                                standingItem.setDriver(carIdx);

                                if (SharedData.isLive)
                                {
                                    standingItem.FastestLap = parseFloatValue(standing, "FastestTime");
                                    standingItem.LapsLed = parseIntValue(standing, "LapsLed");
                                    standingItem.CurrentTrackPct = parseIntValue(standing, "LapsComplete");
                                }
                                standingItem.Laps = new List<LapInfo>();

                                
                                if (SharedData.isLive)
                                {
                                    LapInfo newLap = new LapInfo();
                                    newLap.LapNum = parseIntValue(standing, "LapsComplete");
                                    newLap.LapTime = parseFloatValue(standing, "LastTime");
                                    newLap.Position = parseIntValue(standing, "Position");
                                    newLap.Gap = parseFloatValue(standing, "Time");
                                    newLap.GapLaps = parseIntValue(standing, "Lap");
                                    newLap.SectorTimes = new List<LapInfo.Sector>(3);

                                    standingItem.Laps.Add(newLap);

                                    //Console.WriteLine(standingItem.Driver.Name + " lap " + newLap.LapNum + " added from YAML.");
                                }

                                standingItem.CurrentLap = new LapInfo();
                                standingItem.CurrentLap.LapNum = parseIntValue(standing, "LapsComplete") + 1;

                                SharedData.Sessions.SessionList[sessionIndex].Standings.Add(standingItem);

                                standingItem.NotifyLaps();
                                
                            }
                            else if (SharedData.isLive)
                            {
                                // update item
                                int lapnum = parseIntValue(standing, "LapsComplete");
                                standingItem.FastestLap = parseFloatValue(standing, "FastestTime");
                                standingItem.LapsLed = parseIntValue(standing, "LapsLed");
                                standingItem.NotifySelf();

                                if (SharedData.Sessions.CurrentSession.State == iRTVO.Sessions.SessionInfo.sessionState.cooldown)
                                {
                                    standingItem.CurrentLap.Gap = parseFloatValue(standing, "Time");
                                    standingItem.CurrentLap.GapLaps = parseIntValue(standing, "Lap");
                                    standingItem.CurrentLap.Position = parseIntValue(standing, "Position");
                                    standingItem.CurrentLap.LapNum = parseIntValue(standing, "LapsComplete");
                                }
                            }
                        }
                    }
                }
            }

            // get qualify results if race session standigns is empty
            foreach (Sessions.SessionInfo session in SharedData.Sessions.SessionList)
            {
                if (session.Type == iRTVO.Sessions.SessionInfo.sessionType.race && session.Standings.Count < 1)
                {
                    length = yaml.Length;
                    start = yaml.IndexOf("QualifyResultsInfo:\n", 0, length);

                    // if found
                    if (start >= 0)
                    {
                        end = yaml.IndexOf("\n\n", start, length - start);

                        string QualifyResults = yaml.Substring(start, end - start);

                        length = QualifyResults.Length;
                        start = QualifyResults.IndexOf(" Results:\n", 0, length);
                        end = length;

                        string Results = QualifyResults.Substring(start, end - start - 1);
                        string[] resultList = Results.Split(new string[] { "\n - " }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string result in resultList)
                        {
                            if (result != " Results:")
                            {
                                Sessions.SessionInfo.StandingsItem standingItem = new Sessions.SessionInfo.StandingsItem();
                                standingItem.setDriver(parseIntValue(result, "CarIdx"));
                                standingItem.Position = parseIntValue(result, "Position") + 1;
                                session.Standings.Add(standingItem);
                            }
                        }
                    }
                }
            }

            length = yaml.Length;
            start = yaml.IndexOf("CameraInfo:\n", 0, length);
            end = yaml.IndexOf("\n\n", start, length - start);

            string CameraInfo = yaml.Substring(start, end - start);

            length = CameraInfo.Length;
            start = CameraInfo.IndexOf(" Groups:\n", 0, length);
            end = length;

            string Cameras = CameraInfo.Substring(start, end - start - 1);
            string[] cameraList = Cameras.Split(new string[] { "\n - " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string camera in cameraList)
            {
                int cameraNum = parseIntValue(camera, "GroupNum");
                if (cameraNum < Int32.MaxValue)
                {
                    CameraInfo.CameraGroup camgrp = SharedData.Camera.FindId(cameraNum);
                    //int index = SharedData.Camera.Groups.FindIndex(c => c.Id.Equals(cameraNum));
                    //if (index < 0)
                    if(camgrp.Id < 0)
                    {
                        CameraInfo.CameraGroup cameraGroupItem = new CameraInfo.CameraGroup();
                        cameraGroupItem.Id = cameraNum;
                        cameraGroupItem.Name = parseStringValue(camera, "GroupName");
                        SharedData.Camera.Groups.Add(cameraGroupItem);
                    }
                }
            }

            length = yaml.Length;
            start = yaml.IndexOf("WeekendInfo:\n", 0, length);
            end = yaml.IndexOf("\n\n", start, length - start);

            string WeekendInfo = yaml.Substring(start, end - start);
            SharedData.Track.length = (Single)parseDoubleValue(WeekendInfo, "TrackLength", "km") * 1000;
            SharedData.Track.id = parseIntValue(WeekendInfo, "TrackID");

            IniFile trackNames;

            string filename = Directory.GetCurrentDirectory() + "\\themes\\" + SharedData.theme.name + "\\tracks.ini";
            if (!File.Exists(filename))
                filename = Directory.GetCurrentDirectory() + "\\tracks.ini";

            if (File.Exists(filename))
            {
                trackNames = new IniFile(filename);
                SharedData.Track.name = trackNames.IniReadValue("Tracks", SharedData.Track.id.ToString());
            }

            SharedData.Sessions.SessionId = parseIntValue(WeekendInfo, "SessionID");
            SharedData.Sessions.SubSessionId = parseIntValue(WeekendInfo, "SubSessionID");

            length = yaml.Length;
            start = yaml.IndexOf("SplitTimeInfo:\n", 0, length);
            end = yaml.IndexOf("\n\n", start, length - start);

            string SplitTimeInfo = yaml.Substring(start, end - start);

            length = SplitTimeInfo.Length;
            start = SplitTimeInfo.IndexOf(" Sectors:\n", 0, length);
            end = length;

            string Sectors = SplitTimeInfo.Substring(start, end - start - 1);
            string[] sectorList = Sectors.Split(new string[] { "\n - " }, StringSplitOptions.RemoveEmptyEntries);

            if (sectorList.Length != SharedData.Sectors.Count)
            {
                SharedData.Sectors.Clear();
                foreach (string sector in sectorList)
                {
                    int sectornum = parseIntValue(sector, "SectorNum");
                    if (sectornum < 100)
                    {
                        float sectorborder = parseFloatValue(sector, "SectorStartPct");
                        SharedData.Sectors.Add(sectorborder);
                    }
                }

                // load sectors
                IniFile sectorsIni = new IniFile(Directory.GetCurrentDirectory() + "\\sectors.ini");
                string sectorValue = sectorsIni.IniReadValue("Sectors", SharedData.Track.id.ToString());
                string[] selectedSectors = sectorValue.Split(';');
                Array.Sort(selectedSectors);

                SharedData.SelectedSectors.Clear();

                foreach (string sector in selectedSectors)
                {
                    float number;
                    if (Single.TryParse(sector, out number))
                    {
                        SharedData.SelectedSectors.Add(number);
                    }
                }
            }
        }

        public void getData()
        {
            Single[] DriversTrackPct;
            Int32[] DriversLapNum;
            Int32[] DriversTrackSurface;
            Int32 skip = 0;
            Int32 framenum = 0;
            Int32 prevframenum = 0;
            Int32 livecheck = 0;
            Boolean playing = false;
            Int32 position = 0;

            /*
            iRacingSDK sdk = new iRacingSDK();
            int lastUpdate = -1;
            */

            //TextWriter tw = new StreamWriter("output.txt");

            while (true)
            {
                //Check if the SDK is connected
                if (sdk.IsConnected())
                {
                    // wait
                    SharedData.readMutex.WaitOne(16);
                    SharedData.writeMutex = new Mutex(true);

                    while (sdk.GetData("SessionNum") == null)
                    {

                    }

                    int newUpdate = sdk.Header.SessionInfoUpdate;
                    if (newUpdate != lastUpdate)
                    {
                        lastUpdate = newUpdate;
                        parser(sdk.GetSessionInfo());
                    }

                    framenum = (int)sdk.GetData("ReplayFrameNum");

                    // hide ui if needed
                    if (SharedData.showSimUi == false)
                    {
                        int currentCamState = (int)sdk.GetData("CamCameraState");
                        if ((currentCamState & 0x0008) == 0)
                        {
                            sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.CamSetState, (currentCamState | 0x0008), 0);
                        }
                    }

                    SharedData.Sessions.setCurrentSession((int)sdk.GetData("ReplaySessionNum"));
                    SharedData.Sessions.CurrentSession.Time = (Double)sdk.GetData("SessionTime");
                    SharedData.Sessions.CurrentSession.setFollowedDriver((int)sdk.GetData("CamCarIdx"));

                    // check if watching replay instead of spectating
                    if (livecheck == 0)
                    {
                        position = (int)sdk.GetData("ReplayFrameNum");
                        playing = (Boolean)sdk.GetData("IsReplayPlaying");

                        //sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySearch, (int)iRSDKSharp.ReplaySearchModeTypes.ToEnd, 0);

                        //Thread.Sleep(100);

                        if ((SharedData.Sessions.SessionList[(int)sdk.GetData("ReplaySessionNum")].SessionLength - (Double)sdk.GetData("ReplaySessionTime")) < 0)
                        {
                            SharedData.isLive = false;
                            Console.WriteLine("Replay");
                            SharedData.Sessions.SessionList.Clear();
                            parser(sdk.GetSessionInfo());
                        }
                        else
                        {
                            SharedData.isLive = true;
                            Console.WriteLine("Live");
                            sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySearch, (int)iRSDKSharp.ReplaySearchModeTypes.ToEnd, 0);
                            sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySetPlaySpeed, 1, 0);
                        }
                        /*
                        Thread.Sleep(100);
                        
                        sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySetPlayPosition, 0, 0, position);
                        if (playing)
                            sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySetPlaySpeed, 1, 0);
                        */
                        livecheck = 1;

                        
                    }
                    else if (livecheck == 1)
                    {
                        if (SharedData.isLive)
                        {
                            sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySearch, (int)iRSDKSharp.ReplaySearchModeTypes.ToEnd, 0);
                        }
                        else
                        {
                            sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySetPlayPosition, 0, 0, position);
                            if (playing)
                                sdk.BroadcastMessage(iRSDKSharp.BroadcastMessageTypes.ReplaySetPlaySpeed, 1, 0);
                        }
                        livecheck = 2;
                    }

                    Sessions.SessionInfo.sessionState prevState = SharedData.Sessions.CurrentSession.State;
                    SharedData.Sessions.CurrentSession.State = (Sessions.SessionInfo.sessionState)sdk.GetData("SessionState");
                    if (prevState != SharedData.Sessions.CurrentSession.State)
                    {
                        Event ev = new Event(
                            Event.eventType.state,
                            SharedData.Sessions.CurrentSession.CurrentReplayPosition,
                            SharedData.Sessions.CurrentSession.FollowedDriver.Driver,
                            "Session state changed to " + SharedData.Sessions.CurrentSession.State.ToString(),
                            SharedData.Sessions.CurrentSession.Type,
                            SharedData.Sessions.CurrentSession.LapsComplete
                        );
                        SharedData.Events.List.Add(ev);

                        // if state changes from racing to checkered update final lap
                        if (SharedData.Sessions.CurrentSession.Type == Sessions.SessionInfo.sessionType.race &&
                            prevState == Sessions.SessionInfo.sessionState.racing && 
                            SharedData.Sessions.CurrentSession.State == Sessions.SessionInfo.sessionState.checkered)
                        {
                            SharedData.Sessions.CurrentSession.FinishLine = (Int32)Math.Ceiling(SharedData.Sessions.CurrentSession.getLeader().CurrentTrackPct);
                        }
                    }

                    Sessions.SessionInfo.sessionFlag prevFlag = SharedData.Sessions.CurrentSession.Flag;
                    Sessions.SessionInfo.sessionStartLight prevLight = SharedData.Sessions.CurrentSession.StartLight;

                    parseFlag(SharedData.Sessions.CurrentSession, (Int32)sdk.GetData("SessionFlags"));

                    // white flag handling
                    if (SharedData.Sessions.CurrentSession.LapsRemaining == 1 || SharedData.Sessions.CurrentSession.TimeRemaining < 0)
                    {
                        SharedData.Sessions.CurrentSession.Flag = Sessions.SessionInfo.sessionFlag.white;
                    }

                    if (prevFlag != SharedData.Sessions.CurrentSession.Flag)
                    {
                        Event ev = new Event(
                            Event.eventType.flag,
                            SharedData.Sessions.CurrentSession.CurrentReplayPosition,
                            SharedData.Sessions.CurrentSession.FollowedDriver.Driver,
                            SharedData.Sessions.CurrentSession.Flag.ToString() + " flag",
                            SharedData.Sessions.CurrentSession.Type,
                            SharedData.Sessions.CurrentSession.LapsComplete
                        );
                        SharedData.Events.List.Add(ev);

                        // yellow manual calc
                        if (SharedData.Sessions.CurrentSession.Flag == Sessions.SessionInfo.sessionFlag.yellow)
                        {
                            SharedData.Sessions.CurrentSession.Cautions++;
                        }
                    }

                    if (prevLight != SharedData.Sessions.CurrentSession.StartLight)
                    {
                        Event ev = new Event(
                            Event.eventType.startlights,
                            SharedData.Sessions.CurrentSession.CurrentReplayPosition,
                            SharedData.Sessions.CurrentSession.FollowedDriver.Driver,
                            "Start lights changed to "+ SharedData.Sessions.CurrentSession.StartLight.ToString(),
                            SharedData.Sessions.CurrentSession.Type,
                            SharedData.Sessions.CurrentSession.LapsComplete
                        );
                        SharedData.Events.List.Add(ev);
                    }

                    if (SharedData.Sessions.CurrentSession.SessionStartTime < 0)
                    {
                        SharedData.Sessions.CurrentSession.SessionStartTime = SharedData.Sessions.CurrentSession.Time;
                        SharedData.Sessions.CurrentSession.CurrentReplayPosition = (Int32)sdk.GetData("ReplayFrameNum");
                    }

                    SharedData.Camera.CurrentGroup = (Int32)sdk.GetData("CamGroupNumber");

                    DriversTrackPct = (Single[])sdk.GetData("CarIdxLapDistPct");
                    DriversLapNum = (Int32[])sdk.GetData("CarIdxLap");
                    DriversTrackSurface = (Int32[])sdk.GetData("CarIdxTrackSurface");

                    if (SharedData.isLive && ((Double)sdk.GetData("SessionTime") - (Double)sdk.GetData("ReplaySessionTime")) > 1.1)
                        SharedData.inReplay = true;
                    else
                        SharedData.inReplay = false;

                    /*
                    if ((Int32)sdk.GetData("ReplayPlaySpeed") > 0)
                    {
                    */
                        for (Int32 i = 0; i < SharedData.Drivers.Count; i++)
                        {
                            Sessions.SessionInfo.StandingsItem driver = SharedData.Sessions.CurrentSession.FindDriver(i);

                            if (framenum > prevframenum)
                            {

                                Double prevpos = driver.PrevTrackPct;
                                Double curpos = DriversTrackPct[i];
                                Single speed = 0;

                                // calculate speed
                                //if (curpos != prevpos)
                                //{
                                if (curpos < 0.1 && prevpos > 0.9) // crossing s/f line
                                {
                                    speed = (Single)((((curpos - prevpos) + 1) * (Double)SharedData.Track.length) * (framenum-prevframenum) * 60);
                                }
                                else
                                {
                                    speed = (Single)(((curpos - prevpos) * (Double)SharedData.Track.length) * (framenum - prevframenum) * 60);
                                }
                                
                                if (Math.Abs(driver.Prevspeed - speed) < 2 && (curpos - prevpos) >= 0) // filter junk
                                {
                                    driver.Speed = speed;
                                }
                                
                                /*
                                driver.Speed = speed;

                                if ((curpos - prevpos) < 0)
                                {
                                    driver.Speed = 0;
                                }
                                */
                                driver.Prevspeed = speed;
                                driver.PrevTrackPct = DriversTrackPct[i];
                                //}

                                // update track position
                                if (driver.Finished == false && (Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] != Sessions.SessionInfo.StandingsItem.SurfaceType.NotInWorld)
                                {
                                    driver.CurrentTrackPct = DriversLapNum[i] + DriversTrackPct[i] - 1;
                                }

                                // add new lap
                                if (curpos < 0.1 && prevpos > 0.9 && driver.Finished == false) // crossing s/f line
                                {
                                    if ((Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] != Sessions.SessionInfo.StandingsItem.SurfaceType.NotInWorld &&
                                        speed > 0)
                                    {
                                        //Console.WriteLine(driver.Driver.Name + " finished lap "+ driver.CurrentLap.LapNum +", gap: "+ driver.CurrentLap.Gap + " gaplaps: " + driver.CurrentLap.GapLaps);

                                        DateTime now = DateTime.Now.AddMilliseconds(-curpos / (curpos + (1 - prevpos)));

                                        //Console.WriteLine("cor: " + (-curpos / travel) +"ms");

                                        LapInfo.Sector sector = new LapInfo.Sector();
                                        sector.Num = driver.Sector;
                                        sector.Speed = driver.Speed;
                                        sector.Time = (Single)(now - driver.SectorBegin).TotalSeconds;
                                        sector.Begin = driver.SectorBegin;

                                        if (driver.Laps.Count > 0)
                                        {
                                            driver.CurrentLap.Gap = driver.PreviousLap.Gap;
                                            driver.CurrentLap.GapLaps = driver.PreviousLap.GapLaps;
                                            driver.CurrentLap.SectorTimes.Add(sector);
                                        }

                                        driver.CurrentLap.LapTime = (Single)(now - driver.Begin).TotalSeconds;
                                        //Console.WriteLine(driver.Driver.Name + " unofficial laptime " + driver.CurrentLap.LapTime + ", lap: " + driver.CurrentLap.LapNum);

                                        if (driver.CurrentLap.LapNum > 0 &&
                                            driver.Laps.FindIndex(l => l.LapNum.Equals(driver.CurrentLap.LapNum)) == -1 &&
                                            (SharedData.Sessions.CurrentSession.State != Sessions.SessionInfo.sessionState.gridding ||
                                            SharedData.Sessions.CurrentSession.State != Sessions.SessionInfo.sessionState.cooldown))
                                        {
                                            driver.Laps.Add(driver.CurrentLap);
                                            //Console.WriteLine(driver.Driver.Name + " lap " + driver.CurrentLap.LapNum + "/" + driver.Laps.Count);
                                        }
                                        /*
                                        else
                                        {
                                            Console.WriteLine(driver.Driver.Name + " lap "+ driver.CurrentLap.LapNum + " already added!");
                                        }
                                        */

                                        if (!SharedData.isLive && (driver.CurrentLap.LapTime < driver.FastestLap || driver.FastestLap < 1))
                                        {
                                            driver.FastestLap = driver.CurrentLap.LapTime;
                                        }

                                        driver.CurrentLap = new LapInfo();
                                        driver.CurrentLap.LapNum = DriversLapNum[i];
                                        driver.CurrentLap.Position = driver.Position;
                                        driver.CurrentLap.Gap = driver.PreviousLap.Gap;
                                        driver.CurrentLap.GapLaps = driver.PreviousLap.GapLaps;

                                        driver.SectorBegin = now;
                                        driver.Sector = 0;
                                        driver.Begin = now;


                                        // caution lap calc
                                        if (SharedData.Sessions.CurrentSession.Flag == Sessions.SessionInfo.sessionFlag.yellow && driver.Position == 1)
                                        {
                                            SharedData.Sessions.CurrentSession.CautionLaps++;
                                        }
                                    }
                                }

                                // add sector times
                                if (SharedData.SelectedSectors.Count > 0)
                                {
                                    for (int j = 0; j < SharedData.SelectedSectors.Count; j++)
                                    {
                                        if (curpos > SharedData.SelectedSectors[j] && j > driver.Sector)
                                        {
                                            DateTime now = DateTime.Now.AddMilliseconds(-(curpos - SharedData.SelectedSectors[j]) / (curpos - prevpos));

                                            LapInfo.Sector sector = new LapInfo.Sector();
                                            sector.Num = driver.Sector;
                                            sector.Time = (Single)(now - driver.SectorBegin).TotalSeconds;
                                            sector.Speed = driver.Speed;
                                            sector.Begin = driver.SectorBegin;
                                            driver.CurrentLap.SectorTimes.Add(sector);
                                            driver.SectorBegin = now;
                                            driver.Sector = j;
                                            //Console.WriteLine(driver.Driver.Name + " added sector " + sector.Num + " on lap " + driver.CurrentLap.LapNum + " with time " + sector.Time);
                                        }
                                    }
                                }

                                // cross finish line
                                if (driver.CurrentLap.LapNum + driver.CurrentLap.GapLaps >= SharedData.Sessions.CurrentSession.FinishLine &&
                                    (Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] != Sessions.SessionInfo.StandingsItem.SurfaceType.NotInWorld &&
                                    SharedData.Sessions.CurrentSession.Type == Sessions.SessionInfo.sessionType.race &&
                                    driver.Finished == false)
                                {
                                    // finishing the race
                                    SharedData.Sessions.CurrentSession.UpdatePosition();
                                    driver.CurrentTrackPct = (Math.Floor(driver.CurrentTrackPct) + 0.0064) - (0.0001 * driver.Position);
                                    driver.Finished = true;
                                }

                                // add events
                                if (driver.Driver.CarIdx >= 0)
                                {
                                    // off tracks
                                    if (driver.TrackSurface != (Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] &&
                                        (Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] == Sessions.SessionInfo.StandingsItem.SurfaceType.OffTrack)
                                    {
                                        Event ev = new Event(
                                                Event.eventType.offtrack,
                                                SharedData.Sessions.CurrentSession.CurrentReplayPosition,
                                                driver.Driver,
                                                "Off track",
                                                SharedData.Sessions.CurrentSession.Type,
                                                DriversLapNum[i]
                                            );
                                        SharedData.Events.List.Add(ev);
                                    }

                                    if (driver.TrackSurface != (Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] &&
                                        (Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] == Sessions.SessionInfo.StandingsItem.SurfaceType.NotInWorld)
                                    {
                                        driver.OffTrackSince = DateTime.Now;
                                    }

                                    // pit
                                    if (curpos < SharedData.Sessions.CurrentSession.FinishLine &&
                                        SharedData.Sessions.CurrentSession.Type == Sessions.SessionInfo.sessionType.race)
                                    {

                                        if ((Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] == Sessions.SessionInfo.StandingsItem.SurfaceType.InPitStall &&
                                            (curpos - prevpos) < 5E-08 &&
                                            (DateTime.Now - driver.PitStopBegin).TotalMinutes > 1 &&
                                            /*Math.Abs(driver.Prevspeed - speed) < 2 &&*/
                                            (curpos - prevpos) >= 0)
                                        {
                                            driver.PitStops++;
                                            driver.PitStopBegin = DateTime.Now;
                                            driver.NotifyPit();

                                            Event ev = new Event(
                                                Event.eventType.pit,
                                                SharedData.Sessions.CurrentSession.CurrentReplayPosition,
                                                driver.Driver,
                                                "Pitting on lap " + driver.CurrentLap.LapNum,
                                                SharedData.Sessions.CurrentSession.Type,
                                                driver.CurrentLap.LapNum
                                            );
                                            SharedData.Events.List.Add(ev);
                                        }
                                        else if ((Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] == Sessions.SessionInfo.StandingsItem.SurfaceType.InPitStall &&
                                            (curpos - prevpos) < 5E-08 &&
                                            driver.PitStopBegin > DateTime.MinValue)
                                        {
                                            driver.PitStopTime = (Single)(DateTime.Now - driver.PitStopBegin).TotalSeconds;
                                            driver.NotifyPit();
                                        }
                                        else if ((Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i] == Sessions.SessionInfo.StandingsItem.SurfaceType.InPitStall &&
                                            (curpos - prevpos) > 5E-08 &&
                                            (DateTime.Now - driver.PitStopBegin).TotalMinutes < 1)
                                        {
                                            driver.PitStopTime = (Single)(DateTime.Now - driver.PitStopBegin).TotalSeconds;
                                            driver.NotifyPit();
                                        }
                                    }

                                    // update tracksurface
                                    driver.TrackSurface = (Sessions.SessionInfo.StandingsItem.SurfaceType)DriversTrackSurface[i];
                                }
                            }
                        }

                        if (skip++ >= 20)
                        {
                            SharedData.Sessions.CurrentSession.UpdatePosition();
                            skip = 0;
                        }

                        prevframenum = framenum;
                        SharedData.replayFrameNum = framenum;
                    /*
                    }
                    else
                    {
                        foreach (StandingsItem si in SharedData.Sessions.CurrentSession.Standings)
                        {
                            si.Prevspeed = 0;
                            si.Speed = 0;
                        }
                    }
                    */
                    SharedData.writeMutex.ReleaseMutex();

                    SharedData.apiConnected = true;
                    SharedData.runOverlay = true;
                }
                else if (sdk.IsInitialized)
                {
                    sdk.Shutdown();
                    lastUpdate = -1;
                }
                else
                {
                    sdk.Startup();
                }
                System.Threading.Thread.Sleep(15); //tw.Close();
            }
        }
    }
}