#include "pch.h"
#include <iostream>
#include "string"
#include <fstream>
#include <sstream>
#include "vector"
#include <algorithm>
#include "queue"
#include <set>
#include <map>

using namespace std;

void CreateDotFile(vector<vector<vector<int>>>& matrix, vector<int>& finishStates, vector<int> fromState)
{
	ofstream dotFile("test.dot");
	dotFile << "digraph DeterminatedStateMachine {" << std::endl;
	for (auto& i : fromState)
		if (find(finishStates.begin(), finishStates.end(), i) != finishStates.end())
			dotFile << i << " [shape = box]" << std::endl;
		else
			dotFile << i << std::endl;

	for (size_t i = 0; i < fromState.size(); i++)
		for (size_t j = 0; j < matrix[i].size(); j++)
			if (!matrix[i][j].empty() && matrix[i][j][0] != -1)
				dotFile << "	" << fromState[i] << "->" << matrix[i][j][0] << "[label=" << j << ']' << std::endl;
		
	dotFile << "}";
}

vector<int> setFinishStates(ifstream& input) {
	vector<int> result;
	string str;
	int value;
	getline(input, str);
	getline(input, str);
	stringstream ss(str);

	while (ss >> value)
		result.push_back(value);

	return result;
}

vector<vector<vector<int>>> setMatrix(ifstream& input, const unsigned int z, const unsigned int x) {
	vector<vector<vector<int>>> matrix(z, vector<vector<int>>(x, vector<int>(1, -1)));
	string str;
	int toState = 0, signal = 0, col = 0;
	while (getline(input, str)) {
		stringstream ss(str);

		while (ss >> toState >> signal) {
			if (matrix[col][signal][0] == -1) {
				matrix[col][signal].clear();
			}
			matrix[col][signal].push_back(toState);
		}

		col++;
	}

	return matrix;
}

bool isNotExistElement(vector<int> newVector, int value) {
	return find(newVector.begin(), newVector.end(), value) == newVector.end();
}

void merge(vector<vector<int>>& temp, vector<vector<int>> newVector) {
	for (unsigned int i = 0; i < temp.size(); i++) {
		for (unsigned int j = 0; j < newVector[i].size(); j++) {
			if (newVector[i][j] != -1  && isNotExistElement(temp[i], newVector[i][j])) {
				temp[i].push_back(newVector[i][j]);
				sort(temp[i].begin(), temp[i].end());
			}
		}
	}
}

void restoreMatrix(vector<vector<vector<int>>>& matrix, map<int, vector<int>>& indexStates) {
	for (unsigned int i = 0; i < matrix.size(); i++)
		for (unsigned int j = 0; j < matrix[0].size(); j++)
			for (auto& item : indexStates)
				if (item.second == matrix[i][j]) {
					matrix[i][j].clear();
					matrix[i][j].push_back(item.first);
				}
}

void printOutput(
	ofstream& output,
	vector<vector<vector<int>>>& matrix, 
	vector<int>& finishStates, 
	vector<int>& fromState,
	int x
) {
	output << x << endl;
	output << matrix.size() << endl;
	output << finishStates.size() << endl;
	for (auto& i : finishStates)
		output << i << " ";

	output << endl;
	for (int i = 0; i < fromState.size(); i++) {
		output << fromState[i] << ": ";
		for (int j = 0; j < matrix[i].size(); j++) {
			for (int k = 0; k < matrix[i][j].size(); k++)
				output << matrix[i][j][k] << " ";
		}
		output << endl;
	}
}


void sortMatrix(vector<vector<vector<int>>>& originMatrix) {
	for (int i = 0; i < originMatrix.size(); i++)
		for (int j = 0; j < originMatrix[i].size(); j++)
			sort(originMatrix[i][j].begin(), originMatrix[i][j].end());
}

vector<vector<vector<int>>> getDeterminationMatrix(
	vector<vector<vector<int>>>& originMatrix,
	const int& x,
	vector<int>& finishStates,
	vector<int>& fromState
) {
	sortMatrix(originMatrix);
	vector<vector<vector<int>>> matrix, result;
	set<vector<int>> oldStates;
	vector<vector<int>> temp;
	set<int> newFinishStates;
	vector<int> value;
	queue<vector<int>> queue;
	map<int, vector<int>> indexStates;
	int posInsert = originMatrix.size();

	value.push_back(0);
	queue.push(value);

	while (!queue.empty()) {
		value = queue.front();

		if (find(oldStates.begin(), oldStates.end(), value) == oldStates.end()) {
			if (value.size() == 1) {
				matrix.push_back(originMatrix[value[0]]);
				fromState.push_back(value[0]);

				if (find(finishStates.begin(), finishStates.end(), value[0]) != finishStates.end())
					newFinishStates.insert(value[0]);

				for (unsigned int i = 0; i < x; i++) {
					if (originMatrix[value[0]][i][0] != -1) {
						queue.push(originMatrix[value[0]][i]);
					}
				}
			}
			else {
				temp.clear();
				temp.assign(originMatrix[value[0]].begin(), originMatrix[value[0]].end());

				for (unsigned int i = 1; i < value.size(); i++)
					merge(temp, originMatrix[value[i]]);

				for (int i = 0; i < temp.size(); i++) {
					for (int j = 0; j < temp[i].size(); j++) {
						if (temp[i][j] == -1 && temp[i].size() >= 1) {
							temp[i].erase(temp[i].begin() + j);
						}
					}
				}

				for (auto item : temp)
					if (!item.empty())
						queue.push(item);


				matrix.push_back(temp);
				fromState.push_back(posInsert);
				indexStates[posInsert] = value;
				for (auto item : finishStates)
					if (find(value.begin(), value.end(), item) != value.end())
						newFinishStates.insert(posInsert);
				posInsert++;
			}
		}

		oldStates.insert(value);
		queue.pop();
	}
	
	fromState.erase(unique(fromState.begin(), fromState.end()), fromState.end());
	finishStates.clear();
	finishStates.assign(newFinishStates.begin(), newFinishStates.end());
	restoreMatrix(matrix, indexStates);

	return matrix;
}

int main()
{
	string nameInputFile = "in.txt";
	string nameOutputFile = "out.txt";

	ifstream input(nameInputFile);
	ofstream output(nameOutputFile);

	if (!input.is_open()) {
		cerr << "Error open " << nameInputFile << " file" << endl;
		return 1;
	}

	unsigned int x = 0, z = 0, k = 0, counter = 0;
	vector<int> finishStates, fromState;

	input >> x;
	input >> z;
	input >> k;
	finishStates = setFinishStates(input);
	counter = z;

	vector<vector<vector<int>>> matrix = setMatrix(input, z, x), result;
	result = getDeterminationMatrix(matrix, x, finishStates, fromState);
	printOutput(output, result, finishStates, fromState, x);
	CreateDotFile(result, finishStates, fromState);
}