// appEmbedClient.js
// Copyright (c) 2001 - 2010 Citrix Systems, Inc. All Rights Reserved.

function CreateJICAApplet(Code, Width, Height, CodeBase, Archive, ICAFile, CloseURL, Lang, JICACookie, NoJavaAppletString, UseZeroLatency)
{
    document.write('<applet code="' + Code + '" width="'+Width+'" height="'+Height+'" codebase="'+CodeBase+'" archive="'+Archive+'" name="javaclient" mayscript>');
    document.write('<param name=icafile value="'+ICAFile+'">\n');
    document.write('<param name="Start" value="auto">\n');
    document.write('<param name="End" value="'+CloseURL+'">\n');
    document.write('<param name=Language value="'+Lang+'">\n');
    document.write(JICACookie+'\n ');

    if (UseZeroLatency) {
        document.write('<param name="ZLKeyboardMode" value="2">\n');
    }

    document.write(NoJavaAppletString);
    document.write('<\/applet>');
}

function CreateRDPControl(CLSID, ObjectID, WIDTH, HEIGHT)
{
    document.write('<object type = "text/javascript" classid="' + CLSID + '" id="' + ObjectID +'" width="' + WIDTH + '" height="' + HEIGHT + '"><\/object>');
}
