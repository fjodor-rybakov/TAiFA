#include "pch.h"
#include "iostream"
#include "fstream"
#include "string"
#include "vector"
#include "sstream"
#include "algorithm"

using namespace std;

vector<vector<pair<int, int>>> getMatrix(ifstream& input, int countStates, int countSignals) 
{
	vector<vector<pair<int, int>>> matrix(countSignals, vector<pair<int, int>>(countStates));
	int col = 0, row = 0, state = 0, signal = 0;

	while (input >> state >> signal)
	{
		matrix[col][row] = make_pair(state, signal);
		row++;

		if (row % countStates == 0)
		{
			col++;
			row = 0;
		}
	}

	for (auto i : matrix) {
		for (auto j : i) {
			cout << j.first << " " << j.second << " ";
		}
		cout << endl;
	}

	return matrix;
}

vector<vector<int>> getNewClasses(
	vector<vector<int>>& allSignals, 
	vector<vector<int>>& signals
) // разбиваем на классы эквивалентности
{
	vector<vector<int>> allClasses;
	vector<int> allPos;

	for (auto item : signals)
	{
		allPos.clear();
		auto iter = allSignals.begin();

		while ((iter = find(iter, allSignals.end(), item)) != allSignals.end())
		{
			int index = distance(allSignals.begin(), iter);
			allPos.push_back(index);
			iter++;
		}

		allClasses.push_back(allPos);
	}

	return allClasses;
}

void setStartMatrixSignals(
	vector<vector<pair<int, int>>>& matrix, 
	vector<vector<int>>& allSignals, 
	vector<vector<int>>& unicSignals
) // отбираем все сигналы и уникальные 
{
	vector<int> temp;

	for (auto items : matrix)
	{
		temp.clear();

		for (auto item : items)
			temp.push_back(item.second);

		if (!(find(unicSignals.begin(), unicSignals.end(), temp) != unicSignals.end()))
			unicSignals.push_back(temp);

		allSignals.push_back(temp);
	}
}

void setMatixSignals(
	vector<vector<int>>& newMatrix, 
	vector<vector<int>>& allSignals, 
	vector<vector<int>>& unicSignals, 
	vector<int>& acceptPosition
) // формируем сигналы состояний
{
	vector<int> temp;

	for (unsigned int i = 0; i < newMatrix.size(); i++)
	{
		temp.clear();

		for (unsigned int j = 0; j < newMatrix[i].size(); j++)
			temp.push_back(newMatrix[i][j]);

		if (!(find(unicSignals.begin(), unicSignals.end(), temp) != unicSignals.end()))
		{
			unicSignals.push_back(temp);
			acceptPosition.push_back(i);
		}

		allSignals.push_back(temp);
	}
}

void printMatrix(vector<vector<pair<int, int>>>& matrix, vector<int>& acceptPosition, ofstream& output) 
{
	for (unsigned int i = 0; i < acceptPosition.size(); i++) 
	{
		for (unsigned int j = 0; j < matrix[i].size(); j++) 
			output << matrix[i][j].first << " " << matrix[i][j].second << " ";
		output << endl;
	}
}

int findClass(
	vector<vector<int>> allClasses, 
	int index
) // находим в каком классе эквивалентности находится состояние
{
	int pos = -1;

	for (unsigned int i = 0; i < allClasses.size(); i++) 
		for (unsigned int j = 0; j < allClasses[i].size(); j++) 
			if (allClasses[i][j] == index) return i;

	return pos;
}

void setNewMatrix(
	vector<vector<pair<int, int>>>& matrix, 
	vector<vector<int>>& allClasses, 
	vector<vector<int>>& newMatrix
) // формируем новое состояние по входным данным
{
	int eqClass;

	for (unsigned int i = 0; i < matrix.size(); i++)
		for (unsigned int j = 0; j < matrix[i].size(); j++)
		{
			eqClass = findClass(allClasses, matrix[i][j].first);
			if (eqClass != -1) newMatrix[i][j] = eqClass;
		}
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

	int typeAutomate = 2, countStates = 0, countY = 0, countSignals = 0;
	input >> typeAutomate;
	input >> countStates;
	input >> countY;
	input >> countSignals;

	vector<vector<pair<int, int>>> matrix = getMatrix(input, countStates, countSignals);
	vector<vector<int>> allSignals, unicSignals;

	setStartMatrixSignals(matrix, allSignals, unicSignals);

	vector<vector<int>> allClasses = getNewClasses(allSignals, unicSignals);
	vector<vector<int>> newMatrix(countSignals, vector<int>(countStates));
	vector<int> acceptPosition;
	int countClasses = 0;

	while (true) // пока кол-во состояний не будет равно предыдущему
	{
		allClasses = getNewClasses(allSignals, unicSignals);
		acceptPosition.clear();

		setNewMatrix(matrix, allClasses, newMatrix);

		allSignals.clear();
		unicSignals.clear();

		setMatixSignals(newMatrix, allSignals, unicSignals, acceptPosition);

		if (countClasses == allClasses.size()) break; // выход

		countClasses = allClasses.size();
	}

	printMatrix(matrix, acceptPosition, output);

	return 0;
}