// minMilli.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//

#include "pch.h"
#include <iostream>
#include "fstream"
#include "string"
#include "vector"
#include <sstream>
#include <algorithm>

using namespace std;


vector<string> split(const string& str, char delimiter)
{
	vector<string> tokens;
	string token;
	istringstream tokenStream(str);
	while (getline(tokenStream, token, delimiter))
	{
		tokens.push_back(token);
	}
	return tokens;
}

int getValue(string& val) {
	val.erase(val.begin());
	int value = atoi(val.c_str());
	return value;
}

vector<vector<pair<int, int>>> getMatrix(ifstream& input, int count_states, int count_signals) {
	string value = "";
	vector<vector<pair<int, int>>> matrix(count_signals, vector<pair<int, int>>(count_states));
	int col = 0, row = 0;

	while (input >> value)
	{
		auto values = split(value, '/');
		matrix[col][row] = make_pair(getValue(values[0]) - 1, atoi(values[1].c_str()));
		row++;
		if (row % count_states == 0) {
			col++;
			row = 0;
		}
	}

	return matrix;
}

vector<vector<int>> getNewClasses(vector<vector<int>>& allSignals, vector<vector<int>>& signals)
{
	vector<vector<int>> allClasses;
	vector<int> allPos;

	for (auto k : signals) {
		allPos.clear();
		auto iter = allSignals.begin();
		while ((iter = find(iter, allSignals.end(), k)) != allSignals.end()) {
			int index = distance(allSignals.begin(), iter);
			allPos.push_back(index);
			iter++;
		}
		allClasses.push_back(allPos);
	}

	return allClasses;
}

void printMatrix(vector<vector<pair<int, int>>>& matrix, vector<int>& acceptPosition, ofstream& output) {
	for (unsigned int i = 0; i < acceptPosition.size(); i++) {
		for (unsigned int j = 0; j < matrix[i].size(); j++) {
			output << "q" + to_string(matrix[i][j].first) << "/" << matrix[i][j].second << "\t";
		}
		output << endl;
	}
}

int findClass(vector<vector<int>> allClasses, int index) {
	int pos = -1;

	for (unsigned int i = 0; i < allClasses.size(); i++) {
		for (unsigned int j = 0; j < allClasses[i].size(); j++) {
			if (allClasses[i][j] == index) {
				return i;
			}
		}
	}

	return pos;
}

int main()
{
	string nameInputFile = "input_data.txt";
	string nameOutputFile = "output_data.txt";

	ifstream input(nameInputFile);
	ofstream output(nameOutputFile);

	if (!input.is_open()) 
	{
		cerr << "Error open " << nameInputFile << " file" << endl;
		return 1;
	}

	int count_states = 0, count_signals = 0;
	input >> count_states >> count_signals;
	vector<vector<pair<int, int>>> matrix = getMatrix(input, count_states, count_signals);
	vector<vector<int>> allSignals, signals;

	vector<int> temp;
	for (auto i : matrix) {
		temp.clear();

		for (auto j : i) {
			temp.push_back(j.second);
		}

		if (!(find(signals.begin(), signals.end(), temp) != signals.end())) {
			signals.push_back(temp);
		}

		allSignals.push_back(temp);
	}

	vector<vector<int>> allClasses = getNewClasses(allSignals, signals);
	vector<vector<int>> newMatrix(count_signals, vector<int>(count_states));
	vector<int> acceptPosition;
	int countClasses = 0;

	while (true) {
		allClasses = getNewClasses(allSignals, signals);
		acceptPosition.clear();
		int eqClass;

		for (unsigned int i = 0; i < matrix.size(); i++) {
			for (unsigned int j = 0; j < matrix[i].size(); j++) {
				eqClass = findClass(allClasses, matrix[i][j].first);
				if (eqClass != -1) {
					newMatrix[i][j] = eqClass;
				}
			}
		}

		allSignals.clear();
		signals.clear();
		for (unsigned int i = 0; i < newMatrix.size(); i++) {
			temp.clear();

			for (unsigned int j = 0; j < newMatrix[i].size(); j++) {
				temp.push_back(newMatrix[i][j]);
			}

			if (!(find(signals.begin(), signals.end(), temp) != signals.end())) {
				signals.push_back(temp);
				acceptPosition.push_back(i);
			}

			allSignals.push_back(temp);
		}

		if (countClasses == allClasses.size()) {
			break;
		}

		countClasses = allClasses.size();
	}

	printMatrix(matrix, acceptPosition, output);

	return 0;
}