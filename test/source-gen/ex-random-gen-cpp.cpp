#include <fstream>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string>
#include <set>
#include <vector>
#include <chrono>
#include <thread>
#include <mutex>


using namespace std;

typedef string Node;
typedef string Edge;

set<Node> choiceset;
set<Edge> edgeSet;
int edgeCount;
mutex globalMutex;

void genEdge(string argvEdge, int nodeCount) {
    while (edgeCount < stoi(argvEdge)) {
        auto iterator1 = choiceset.begin();
        auto iterator2 = choiceset.begin();
        int idx1 = rand() % nodeCount;
        int idx2 = rand() % nodeCount;
        for (int i = 0; i < idx1 && i < nodeCount - 2; i++)
            ++iterator1;
        for (int i = 0; i < idx2 && i < nodeCount - 2; i++)
            ++iterator2;

        Node node1 = *iterator1;
        Node node2 = *iterator2;
        if (node1 != node2) {
            Edge newEdge = node1 + " " + node2;
            Edge revEdge = node2 + " " + node1;
            if (edgeSet.find(newEdge) == edgeSet.end() && edgeSet.find(revEdge) == edgeSet.end()) {
                // globalMutex.lock();
                edgeSet.insert(newEdge);
                edgeCount++;
                // globalMutex.unlock();
            }
        }
    }
}


int main(int argc, char const *argv[]) {
    if (argc < 3) {
        fprintf(stderr, "Usage : program <node> <edge>\n");
        exit(1);
    }

    srand(time(NULL));
    ofstream targetFile = ofstream("Ex-graph.txt");

    char alphabet[26];
    for (int i = 0; i < 26; i++)
        alphabet[i] = (char) i + 0x41;

    std::chrono::steady_clock::time_point beginNode = std::chrono::steady_clock::now();
    cout << "Generating nodes...\n";

    int nodeCount = 0;
    while (nodeCount < stoi(argv[1])) {
        bool isAdded = false;
        Node newNode = "";
        while (not isAdded) {
            newNode = newNode + alphabet[rand() % 26];
            if (choiceset.find(newNode) == choiceset.end()) {
                choiceset.insert(newNode);
                nodeCount++;
                isAdded = true;
            }
        }
    }
    std::chrono::steady_clock::time_point endNode = std::chrono::steady_clock::now();

    std::chrono::steady_clock::time_point beginEdge = std::chrono::steady_clock::now();
    cout << "Generating edges...\n";
    edgeCount = 0;
    thread t1(genEdge, argv[2], nodeCount);
    // thread t2(genEdge, argv[2], nodeCount);

    t1.join();
    // t2.join();
    std::chrono::steady_clock::time_point endEdge = std::chrono::steady_clock::now();

    cout << "Writing...\n";
    targetFile << edgeCount << endl;
    for (auto it = edgeSet.begin(); it != edgeSet.end(); ++it) {
        string rowentry = *it + "\n";
        targetFile << rowentry;
    }

    std::cout << "Time elapsed gen-nodes : " << (double) std::chrono::duration_cast<std::chrono::microseconds>(endNode - beginNode).count()/1000000 << " sec" << std::endl;
    std::cout << "Time elapsed gen-edges : " << (double) std::chrono::duration_cast<std::chrono::microseconds>(endEdge - beginEdge).count()/1000000 << " sec" << std::endl;

    targetFile.close();
    return 0;
}
