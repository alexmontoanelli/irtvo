# Introduction #

Remote controlling allows users to use iRTVO over a network (even over the Internet). There are multiple reasons why to use remote controlling. For example user can have separate machine for streaming that is operated over the local network or the Internet. Another possible scenario is that the director acts as a server and commentators connect to him to see exactly what is displayed to viewers.

# Setting up server #

Server is the most critical part of the network. Server waits clients to connect and delivers messages between them. Machine that is most hard to reach (physically) should be set as the server. This is because if client drops out, user has to manually reconnect it. Server on the other hand just keeps waiting for client to reconnect and doesn't need any human intervention.

As long as it doesn't crash, obviously.

To setup server open options window and set a TCP port, 11107 is the default. Any random port between 1025-65535 is fine. Program generates random key for the server, change it if you want, but use **only** numbers and letters. Key is used for letting some of the clients to control server. Check automatic start if you want server to start automatically when iRTVO is started.

Now click server button from the main window. Click it again and server will close.

Next poke a hole in your firewall for the port you specified. If you use windows' firewall you are being prompted if you want to do that. If you are using NAT on your LAN you must do port forwarding. Refer your router's manual how to do that.

# Setting up client #

Clients connect to server and receive or send messages to it. If the server owner gives the client the right key, client can control server's overlays, otherwise client will just receive whatever server does. There's a special button for client which is follow server. By default it is set when ever client first connect to server. When following is set client executes every command that server sends to it. If following is disabled client will queue the commands it receives and execute them when following is enabled again.

To setup client open options and set servers IP-address, port and key (optional). Check automatic start if you want client to automatically connect when iRTVO is started.

After that you can connect to server clicking Client button from main window. Clicking it twice makes the client to disconnect.

If you used the right key you should be able to control the server. Otherwise, you will just follow whatever the server is doing. To stop following server commands click "Stop following"-button and to resume following click "Follow server".

# Caveats #

  * When connected client's and server's overlays should be in same state. Using "hide all" and live buttons is sure way to sync them up.
  * If server goes into replay while client is not following it (queuing commands). This can lead to de-sync which is resolved when server and client both return back to live (click live button).