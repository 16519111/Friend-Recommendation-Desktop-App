filename = input("Enter filename > ")
f = open(filename).readlines()

processed = []
for i in range(len(f)):
    if i == 0:
        continue
    else:
        processed.append(f[i].strip().split(" "))

print("Edge count :", len(processed))

isRun = True
while isRun:
    print("\n\nMenu")
    print("1. Find edge with node")
    print("2. Find mutual")
    print("3. Exit")
    type = int(input("Input menu number > "))
    if type == 1:
        node1 = input("N1 | ")
        node2 = input("N2 | ")
        isFound = False
        for i in range(len(processed)):
            if node1 in processed[i] and node2 in processed[i]:
                isFound = True
                print(processed[i], " edge found in line ", i+2) # + 2 due starting from 0 and ignoring edge count at top file
                break
        if not isFound:
            print("Edge not found")
    elif type == 2:
        node1 = input("N1 | ")
        isFound = False
        for i in range(len(processed)):
            if node1 in processed[i]:
                isFound = True
                print(processed[i], "at Line ",i+2) # + 2 due starting from 0 and ignoring edge count at top file
        if not isFound:
            print("Node not found")
    else:
        isRun = False
