import matplotlib.pyplot as plt
import matplotlib.animation as animation
from matplotlib import style
import numpy as np

style.use('fivethirtyeight')

# --- Setup the figure and subplots ------------------------------------
fig, ((ax1, ax2), (ax3, ax4)) = plt.subplots(2, 2)

# --- read the data ----------------------------------------------------
def read_data_colorVisits():
    with open("colorVisits.csv", 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    for line in graph_data:
        print(line)
    heights = [int(line.strip()) for line in graph_data if line.strip()]
    return heights

def read_data_noColorTime():
    with open("avgNoColorTime.csv", 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    time_series = [float(line.strip().replace(",",".")) for line in graph_data if line.strip()]
    return time_series

def read_data_avgDistToCentre():
    with open("avgDistToCentre.csv", 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    heights = [float(line.strip().replace(",",".")) for line in graph_data if line.strip()]
    return heights

def read_data_avgSpeed():
    with open("avgSpeed.csv", 'r') as file:
        graph_data = file.readlines()
    graph_data = graph_data[1:]
    heights = [float(line.strip().replace(",",".")) for line in graph_data if line.strip()]
    return heights

# --- Define the plots ------------------------------------------------------
def animate_colorVisits(i):
    heights = read_data_colorVisits()
    bar_labels = ['white-white', 'white-black', 'black-white', 'black-black']

    if len(heights) != len(bar_labels):
        print(f"Error: Number of data points ({len(heights)}) does not match number of labels ({len(bar_labels)}).")
        return

    ax1.clear()
    ax1.bar(range(len(heights)), heights, tick_label=bar_labels)

    ax1.set_title('Color visits frequency')
    ax1.set_xlabel('Color visits')
    ax1.set_ylabel('Frequency')

def animate_noColorTime(i):
    heights = read_data_noColorTime()
    ax2.clear()
    ax2.plot(range(len(heights)), heights)
    ax2.set_title('Average time between color changes')
    ax2.set_xlabel('Time')
    ax2.set_ylabel('Average travel time')

def animate_avgDistToCentre(i):
    distances = read_data_avgDistToCentre()
    ax3.clear()
    x = range(len(distances))
    y = np.array(distances)

    ax3.plot(x, y, 'k-', label='y')
    ax3.fill_between(x, y, where=(y < 1), color='green', alpha=0.5, interpolate=True) # color background
    ax3.fill_between(x, y, where=(y >= 1), color='red', alpha=0.5, interpolate=True)

    ax3.set_ylim([0, 4]) # hardcode ylim 4 so we can better see if value is high or low

    ax3.set_title('Average distance of a robot to swarm centre')
    ax3.set_xlabel('Time')
    ax3.set_ylabel('Average distance')

# Define the animation function for the fourth subplot
def animate_avgSpeed(i):
    distances = read_data_avgSpeed()
    ax4.clear()
    x = range(len(distances))
    y = np.array(distances)

    ax4.plot(x, y, 'k-', label='y')
    ax4.fill_between(x, y, where=(y < 0.20), color='blue', alpha=0.5, interpolate=True) # color background
    ax4.fill_between(x, y, where=(y >= 0.20), color='yellow', alpha=0.5, interpolate=True)

    ax4.set_ylim([0, 0.31]) # hardcode ylim 4 so we can better see if value is high or low

    ax4.set_title('Average speed in the swarm')
    ax4.set_xlabel('Time')
    ax4.set_ylabel('Average speed')

# Set up the animation
ani1 = animation.FuncAnimation(fig, animate_colorVisits,        interval=1000)
ani2 = animation.FuncAnimation(fig, animate_noColorTime,        interval=1000)
ani3 = animation.FuncAnimation(fig, animate_avgDistToCentre,    interval=1000)
ani4 = animation.FuncAnimation(fig, animate_avgSpeed,           interval=1000)

plt.tight_layout()
plt.show()