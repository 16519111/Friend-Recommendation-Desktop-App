# Extended Random graph generator

import sys
from optparse import OptionParser
import random
import math

ftarget = open("extended-random-graph.txt", "w")

parser = OptionParser()
parser.add_option('-n', '--nodes', default=6,   help='Node count',  action='store', type='int', dest='nodecount')
parser.add_option('-e', '--edges', default=8,   help='Edge count',  action='store', type='int', dest='edgecount')

(options, args) = parser.parse_args()

print("Node :", options.nodecount)
print("Edge :", options.edgecount)

random.seed()
Alphabet = [chr(i+0x41) for i in range(26)]
choicelist = []
print("Generating nodes...")
while len(choicelist) < options.nodecount:
    added = False
    newchoice = str(random.choice(Alphabet))
    while not added:
        if newchoice not in choicelist:
            choicelist.append(newchoice)
            added = True
        else:
            newchoice = newchoice + str(random.choice(Alphabet))


nodecreated = 0
edgecreated = 0
graph = []
nodeselected = []
print("Generating edges...")
while (edgecreated < options.edgecount):
    firstnode = random.choice(choicelist)
    secondnode = random.choice(choicelist)
    if (firstnode != secondnode and [firstnode, secondnode] not in graph and [secondnode, firstnode] not in graph):
        if (firstnode not in nodeselected and nodecreated < options.nodecount):
            nodeselected.append(firstnode)
            nodecreated += 1
        if (secondnode not in nodeselected and nodecreated < options.nodecount):
            nodeselected.append(secondnode)
            nodecreated += 1
        if (firstnode in nodeselected and secondnode in nodeselected):
            graph.append([firstnode, secondnode])
            edgecreated += 1

print("Node selected : ", nodeselected)
ftarget.write(str(options.edgecount) + "\n")
for connection in graph:
    print(connection[0], connection[1])
    ftarget.write(connection[0] + " " + connection[1] + "\n")

ftarget.close()
