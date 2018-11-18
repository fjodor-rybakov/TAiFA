#include "pch.h"
#include <iostream>
#include "string"
#include <fstream>
#include <sstream>
#include "vector"
#include <algorithm>

using namespace std;

vector<int> setFinishStates(ifstream& input) {
	vector<int> result;
	string str; 
	int value;
	getline(input, str);
	getline(input, str);
	stringstream ss(str);
	while (ss >> value) {
		result.push_back(value);
	}
	return result;
}

vector<vector<vector<int>>> setMatrix(ifstream& input, int z, int x) {
	vector<vector<vector<int>>> matrix(z, vector<vector<int>>(x, vector<int>(1, -1)));
	string str;
	int toState = 0, signal = 0, row = 0, col = 0;
	while (getline(input, str)) {
		stringstream ss(str);

		while (ss >> toState >> signal) {
			if (matrix[col][signal][0] == -1)
				matrix[col][signal].clear();
			matrix[col][signal].push_back(toState);
		}

		col++;
		cout << str << endl;
	}

	return matrix;
}

bool isNotExistElement(vector<int> newVector, int value) {
	return find(newVector.begin(), newVector.end(), value) == newVector.end();
}

void merge(vector<vector<int>>& temp, vector<vector<int>> newVector) {
	for (int i = 0; i < temp.size(); i++) {
		for (int j = 0; j < newVector[i].size(); j++) {
			if (newVector[i][j] != -1 && isNotExistElement(temp[i], newVector[i][j])) {
				temp[i].push_back(newVector[i][j]);
			}
		}
	}
}

void printMatrix(vector<vector<int>> matrix) {
	for (auto g : matrix) {
		for (auto h : g) {
			cout << h << " ";
		}
		cout << endl;
	}
}

vector<vector<int>> getDetermMatrix(vector<vector<vector<int>>> matrix) {
	int col = 0;
	vector<vector<vector<int>>> result;
	vector<vector<int>> temp;
	for (int i = 0; i < matrix.size(); i++) {
		for (int j = 0; j < matrix[i].size(); j++) {
			if (matrix[i][j].size() != 1) {
				temp.assign(matrix[matrix[i][j][0]].begin(), matrix[matrix[i][j][0]].end());
				for (int k = 1; k < matrix[i][j].size(); k++) {
					col = matrix[i][j][k];
					merge(temp, matrix[col]);
				}
				result.push_back(temp);
				//printMatrix(temp);
			}
		}
	}

	result.erase(unique(result.begin(), result.end()), result.end());

	for (int i = 0; i < result.size(); i++) {
		for (int j = 0; j < result[i].size(); j++) {
			for (int k = 0; k < result[i][j].size(); k++) {
				cout << result[i][j][k] << " ";
			}
		}
		cout << endl;
	}

	return temp;
}

int main()
{
	string nameInputFile = "in.txt";
	string nameOutputFile = "out.txt";

	ifstream input(nameInputFile);
	ofstream output(nameOutputFile);

	if (!input.is_open())
	{
		cerr << "Error open " << nameInputFile << " file" << endl;
		return 1;
	}

	int x = 0, z = 0, k = 0;
	vector<int> finishStates;

	input >> x;
	input >> z;
	input >> k;
	finishStates = setFinishStates(input);

	vector<vector<vector<int>>> matrix = setMatrix(input, z, x);
	vector<vector<int>> resultMatrix = getDetermMatrix(matrix);

	/*for (auto i : matrix) {
		for (auto j : i) {
			for (int k : j) {
				cout << k << " ";
			}
		}
		cout << endl;
	}*/
}