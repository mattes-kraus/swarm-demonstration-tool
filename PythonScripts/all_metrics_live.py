import os
import matplotlib.pyplot as plt
import matplotlib.animation as animation
from matplotlib import style
import numpy as np

style.use('fivethirtyeight')

# --- Setup the figure and subplots ------------------------------------
fig, ((ax1, ax2), (ax3, ax4)) = plt.subplots(2, 2)
textsize = 14
plt.rcParams.update({
    'font.family': 'Times New Roman',   # Serifen Schrift für LateX-Kompatibilität 
    'axes.titlesize': textsize,         # Titel der Achsen
    'axes.labelsize': textsize,         # Beschriftung der Achsen
    'xtick.labelsize': textsize,        # Tick-Labels der x-Achse
    'ytick.labelsize': textsize,        # Tick-Labels der y-Achse
    'legend.fontsize': textsize,        # Legenden-Schriftgröße
    'figure.titlesize': textsize,       # Titel der Figur})
    'axes.titleweight': 'bold',         # Achsentitel fett
})

# --- read the data ----------------------------------------------------

metric_path = os.path.join(".", "Build", "Metrics_test_instance")

def read_data_colorVisits():
    with open(os.path.join(metric_path, "colorVisits.csv"), 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    heights = [int(line.strip()) for line in graph_data if line.strip()]
    return heights

def read_data_noColorTime():
    with open(os.path.join(metric_path, "avgNoColorTime.csv"), 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    time_series = [float(line.strip().replace(",",".")) for line in graph_data if line.strip()]
    return time_series

def read_data_avgDistToCentre():
    with open(os.path.join(metric_path, "avgDistToCentre.csv"), 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    heights = [float(line.strip().replace(",",".")) for line in graph_data if line.strip()]
    return heights

def read_data_avgSpeed():
    with open(os.path.join(metric_path, "avgSpeed.csv"), 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    heights = [float(line.strip().replace(",",".")) for line in graph_data if line.strip()]
    return heights

# --- Define the plots ------------------------------------------------------
def animate_colorVisits(i):
    heights = read_data_colorVisits()
    bar_labels = ['w-w', 'w-b', 'b-w', 'b-b']

    if len(heights) != len(bar_labels):
        print(f"Error: Number of data points ({len(heights)}) does not match number of labels ({len(bar_labels)}).")
        return

    ax1.clear()
    ax1.bar(range(len(heights)), heights, tick_label=bar_labels)

    ax1.set_title('Color visits frequency')
    ax1.set_ylabel('Frequency')
    ax1.set_xlabel('Color trajectory with w-white, b-black')

def animate_noColorTime(i):
    heights = read_data_noColorTime()
    ax2.clear()
    ax2.plot(range(len(heights)), heights)
    ax2.set_title('Average time between color changes')
    ax2.set_xlabel('Time in s')
    ax2.set_ylabel('Average travel time in s')

def animate_avgDistToCentre(i):
    distances = read_data_avgDistToCentre()
    ax3.clear()
    x = range(len(distances))
    y = np.array(distances)

    ax3.plot(x, y, 'k-', label='Avg dist to swarm centre')
    ax3.fill_between(x, y, where=(y < 1), color='green', alpha=0.5, interpolate=True, label="Dense") # color background
    ax3.fill_between(x, y, where=(y >= 1), color='red', alpha=0.5, interpolate=True, label="Spread")

    ax3.set_ylim([0, 4]) # hardcode ylim 4 so we can better see if value is high or low

    ax3.set_title('Average distance of a robot to swarm centre')
    ax3.set_xlabel('Time in s')
    ax3.set_ylabel('Average distance in m')

    ax3.legend()

def animate_avgSpeed(i):
    distances = read_data_avgSpeed()
    ax4.clear()
    x = range(len(distances))
    y = np.array(distances)

    ax4.plot(x, y, 'k-', label='Avg robot speed')
    ax4.fill_between(x, y, where=(y < 0.20), color='blue', alpha=0.5, interpolate=True, label="Static") # color background
    ax4.fill_between(x, y, where=(y >= 0.20), color='yellow', alpha=0.5, interpolate=True, label="Dynamic")

    ax4.set_ylim([0, 0.35]) # hardcode ylim to little over max speed so we can better see if value is high or low

    ax4.set_title('Average speed in the swarm')
    ax4.set_xlabel('Time in s')
    ax4.set_ylabel('Average speed in m/s')

    ax4.legend()

# Set up the animation so the data updates every second
ani1 = animation.FuncAnimation(fig, animate_colorVisits,        interval=1000)
ani2 = animation.FuncAnimation(fig, animate_noColorTime,        interval=1000)
ani3 = animation.FuncAnimation(fig, animate_avgDistToCentre,    interval=1000)
ani4 = animation.FuncAnimation(fig, animate_avgSpeed,           interval=1000)

plt.tight_layout()
plt.show()