# A Tool for Demonstration and Visualisation of Robot Swarms

A Unity project for a tool, where you can control a swarm of robots and record certain metrics.
Read my report for more information.

<p align="center">
  <img src="Videos/arena_design.gif"/>
</p>

## Setup

- clone the repository (```git clone git@github.com:StudentWorkCPS/swarm-demonstration-tool.git```)
- navigate to Assets/Submodules/ and execute ```git clone https://github.com/jfg8/csDelaunay.git```
- open Unity Hub
- press Add and add the swarm-demonstration-tool
- open the new swarm-demonstration-tool project from Unity Hub (eventually you have to adjust the Unity-Editor version to 2021.3.34f1)
- in the Unity Window, go to "File > Build Settings", select your platform and press Build
- select SwarmDemonstration/Build and wait for the project to build. If the path doesn't
exist, create it. 
- after processing, there is an executable file in /Build.

## Usage
To run the tool, open your explorer and navigate to swarm-demonstration-tool/Build. and run 
SwarmDemonstration.exe

To see the metrics, navigate to the root directory and run ```python3 ./PythonScripts/all_metrics_live.py```.
Keep in mind, that the metrics graphs have no history.

Attention: the tool was developed on Windows. Therefore, I couldn't test the tool on Linux or macOS. 

## Videos
There are videos of all three missions suggested in the report. To view them, go to /Videos and open them
with the tool of your choice, e.g. VLC Media Player.
