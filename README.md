# ErgoBike.io
This is a C# console app that communicates (serial port) with the ergo bike TRS 4008/8008

This is sample code used as part of a research project.
Details about the research project can be found in the following publication:
Juan M. Silva, Abdulmotaleb El Saddik. 2011. Evaluation of an Adaptive Target-Shooting Exergame. In Proceedings of 2nd International Conference on Serious Gaming and 7th Science meets Business Congress (GAMEDAYS 2011), Darmstadt, Germany, September 12-13, 2011.

Please contact me to get a copy of the full paper.

About the code:

This is a Visual Studio solution that runs as a console application. It establishes communication with the ergo bike connected to the PC through a serial port (COM). The bike uses a very specific communication protocol based on writing/reading a series of bytes in certain sequence.

The code runs in a loop setting the watts value (bike ergo resistance) and reading values from the cockpit (Watts, RPM, Seep, Heart Rate, and Gear). 
