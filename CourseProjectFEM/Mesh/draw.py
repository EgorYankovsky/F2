import os
import matplotlib.pyplot as plt
import matplotlib.patches as patches
import numpy as np
from decimal import Decimal as dcm
from matplotlib.widgets import Button
from matplotlib import colors as mcolors
import colorsys

elements = []
points = []
xL, yL = [], []
q = []

fig, ax = plt.subplots(1)

hardPathPc = 'Z:\\work\\F2\\CourseProjectFEM\\Mesh\\'

file1 = "points.txt"
path1 = os.path.join(hardPathPc, file1)
file2 = "elements.txt"
path2 = os.path.join(hardPathPc, file2)
file3 = "q.txt"
path3 = os.path.join(hardPathPc, file3)

with open(path1) as file:
    for line in file:
        xUniq, yUniq = line.split()
        points.append([float(xUniq), float(yUniq)])
        xL.append(float(xUniq))
        yL.append(float(yUniq))

with open(path2) as file:
    for line in file:
        vert1, vert2, vert3, vert4, area = map(int, line.split())
        elements.append([vert1, vert2, vert3, vert4, area])

nx = elements[0][2] - 1
ny = len(elements) // nx
xUniq = []
yUniq = []

for i in range(nx + 1):
    xUniq.append(points[i][0])

for i in range(ny + 1):
    yUniq.append(points[i * (nx + 1)][1])

q = np.zeros([len(points)])

idx = 0

with open(path3) as file:
    for line in file:
        q[idx] = dcm(line)
        idx += 1

qmin = np.min(q)
qmax = np.max(q)

def man_cmap(cmap, value=1.):
    colors = cmap(np.arange(cmap.N))
    hls = np.array([colorsys.rgb_to_hls(*c) for c in colors[:,:3]])
    hls[:,1] *= value
    rgb = np.clip(np.array([colorsys.hls_to_rgb(*c) for c in hls]), 0,1)
    return mcolors.LinearSegmentedColormap.from_list("", rgb)
cmap = plt.cm.get_cmap("jet")
# Создание более блеклой версии jet
lighter_cmap = man_cmap(cmap, 1.1) # Значение 0.75 делает цветовую схему более блеклой

X, Y = np.meshgrid(xUniq, yUniq)
q = np.reshape(q, (len(yUniq), len(xUniq)))
colorBar = plt.contourf(X, Y, q, levels=100, cmap=lighter_cmap, vmin=qmin, vmax=qmax)
plt.colorbar(colorBar, ax=ax, format='%.0e')
for i in range(len(yUniq)):
    ax.axhline(y=yUniq[i], color='white', linestyle='-', linewidth=0.4)

for i in range(len(xUniq)):
    ax.axvline(x=xUniq[i], color='white', linestyle='-', linewidth=0.4)


def draw_magnit():
    linew = 3.5
    outerw = 5.0

    clr = 'black'
    ax.plot([0.0, 0.0], [0.0, 0.06], color=clr, linewidth=linew)
    ax.plot([0.0, 0.1], [0.06, 0.06], color=clr, linewidth=linew)
    ax.plot([0.1, 0.1], [0.0, 0.06], color=clr, linewidth=linew)

    ax.plot([0.02, 0.02], [0.0, 0.04], color=clr, linewidth=linew)
    ax.plot([0.02, 0.04], [0.04, 0.04], color=clr, linewidth=linew)
    ax.plot([0.04, 0.04], [0.002, 0.04], color=clr, linewidth=linew)

    ax.plot([0.025, 0.025], [0.025, 0.035], color=clr, linewidth=linew)
    ax.plot([0.025, 0.035], [0.025, 0.025], color=clr, linewidth=linew)
    ax.plot([0.025, 0.035], [0.035, 0.035], color=clr, linewidth=linew)
    ax.plot([0.035, 0.035], [0.025, 0.035], color=clr, linewidth=linew)

    ax.plot([0.06, 0.06], [0.002, 0.04], color=clr, linewidth=linew)
    ax.plot([0.06, 0.08], [0.04, 0.04], color=clr, linewidth=linew)
    ax.plot([0.08, 0.08], [0.0, 0.04], color=clr, linewidth=linew)

    ax.plot([0.065, 0.065], [0.025, 0.035], color=clr, linewidth=linew)
    ax.plot([0.065, 0.075], [0.025, 0.025], color=clr, linewidth=linew)
    ax.plot([0.065, 0.075], [0.035, 0.035], color=clr, linewidth=linew)
    ax.plot([0.075, 0.075], [0.025, 0.035], color=clr, linewidth=linew)

    ax.plot([0.04, 0.06], [0.002, 0.002], color=clr, linewidth=linew)

    ax.plot([np.min(xUniq), np.max(xUniq)], [np.min(yUniq), np.min(yUniq)], color=clr, linewidth=outerw)
    ax.plot([np.min(xUniq), np.min(xUniq)], [np.min(yUniq), np.max(yUniq)], color=clr, linewidth=outerw)
    ax.plot([np.min(xUniq), np.max(xUniq)], [np.max(yUniq), np.max(yUniq)], color=clr, linewidth=outerw)  
    ax.plot([np.max(xUniq), np.max(xUniq)], [np.min(yUniq), np.max(yUniq)], color=clr, linewidth=outerw)         

draw_magnit()

class Index:
    def prev(self, event):
        for elem in elements:
            if elem[4] == 0:
                color = 'blue'
            elif elem[4] == 1:
                color = 'green'
            elif elem[4] == 2:
                color = 'red'
            elif elem[4] == 3:
                color = 'cyan'

            quadrangle = patches.Polygon([
                [xL[elem[0]], yL[elem[0]]],
                [xL[elem[1]], yL[elem[1]]],
                [xL[elem[3]], yL[elem[3]]],
                [xL[elem[2]], yL[elem[2]]]
            ],
                edgecolor='white', facecolor=color, linewidth=0.3)
            ax.add_patch(quadrangle)


        plt.plot(xL, yL, " ")
        plt.draw()

    def next(self, event):
        ax.cla() # Очистка графика
        for i in range(len(yUniq)):
            ax.axhline(y=yUniq[i], color='white', linestyle='-', linewidth=0.4)

        for i in range(len(xUniq)):
            ax.axvline(x=xUniq[i], color='white', linestyle='-', linewidth=0.4)
        colorBar = ax.contourf(X, Y, q, levels=100, cmap=lighter_cmap, vmin=qmin, vmax=qmax)
        draw_magnit()
        plt.draw()

ax.set_aspect('equal')

callback = Index()
axprev = fig.add_axes([0.34, 0.90, 0.1, 0.05])
bprev = Button(axprev, 'Mesh')
bprev.on_clicked(callback.prev)

axnext = fig.add_axes([0.46, 0.90, 0.1, 0.05])
bnext = Button(axnext, 'Color')
bnext.on_clicked(callback.next)

plt.show()