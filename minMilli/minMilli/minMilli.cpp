#include "pch.h"
#include "iostream"
#include "fstream"
#include "string"
#include "vector"
#include "sstream"
#include "algorithm"

using namespace std;

vector<vector<pair<int, int>>> getMatrix(
	ifstream& input, 
	int countStates, 
	int countX
) // считываем матрицу сохраняя её в перевенутом виде
{
	vector<vector<pair<int, int>>> matrix(countStates, vector<pair<int, int>>(countX));
	int col = 0, row = 0, state = 0, signal = 0;

	while (input >> state >> signal)
	{
		matrix[row][col] = make_pair(state, signal);
		row++;

		if (row % countStates == 0)
		{
			col++;
			row = 0;
		}
	}

	return matrix;
}

void setStartMatrixSignals(
	vector<vector<pair<int, int>>>& matrix,
	vector<vector<int>>& allSignals,
	vector<vector<int>>& allClasses
) // отбираем все сигналы у начальной матрицы, и формируем классы эквивалентности
{
	vector<int> temp, tempPos;
	vector<vector<int>> unicSignals;

	for (unsigned int i = 0; i < matrix.size(); i++) {
		temp.clear();

		for (unsigned int j = 0; j < matrix[i].size(); j++) {
			temp.push_back(matrix[i][j].second);
		}

		tempPos.push_back(i);

		auto pos = find(unicSignals.begin(), unicSignals.end(), temp);

		if (pos == unicSignals.end()) {
			unicSignals.push_back(temp);
			allClasses.push_back(tempPos);
		}
		else {
			auto index = distance(unicSignals.begin(), pos);
			allClasses[index].push_back(i);
		}

		tempPos.clear();

		allSignals.push_back(temp);
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

void setMatixSignals(
	vector<vector<int>>& newMatrix,
	vector<vector<int>>& allSignals,
	vector<vector<int>>& allClasses,
	vector<int>& acceptPosition
) // формируем сигналы состояний
{
	vector<int> temp, tempPos;
	vector<vector<int>> unicSignals, tempNewAllClasses, newAllClasses;

	for (auto items : allClasses)
	{
		tempNewAllClasses.clear();

		for (auto item : items)
		{
			temp.clear();

			for (unsigned int k = 0; k < newMatrix[item].size(); k++)
				temp.push_back(newMatrix[item][k]);

			tempPos.push_back(item);

			auto pos = find(unicSignals.begin(), unicSignals.end(), temp);

			if (pos == unicSignals.end())
			{
				unicSignals.push_back(temp);
				tempNewAllClasses.push_back(tempPos);
				acceptPosition.push_back(item);
			}
			else
			{
				auto index = distance(unicSignals.begin(), pos);
				tempNewAllClasses[index].push_back(item);
			}

			tempPos.clear();
		}
		unicSignals.clear();

		for (auto item : tempNewAllClasses)
			newAllClasses.push_back(item);
	}

	allClasses = newAllClasses;
}

vector<vector<int>> transpose(const vector<vector<int>>& matrix, int countX, int countStates) {
	vector<vector<int>> newMatrix(countX, vector<int>(countStates));

	for (unsigned int i = 0; i < matrix.size(); i++) 
		for (unsigned int j = 0; j < matrix[i].size(); j++)
			newMatrix[j][i] = matrix[i][j];

	return newMatrix;
}
 
void printMatrix(
	vector<vector<pair<int, int>>>& matrix, 
	vector<vector<int>>& newMatrix, 
	vector<int>& acceptPosition, 
	int typeAutomate,
	int countX,
	int countY,
	int countStates,
	ofstream& output
)
{
	vector<vector<int>> newTransposeMatrix = transpose(newMatrix, countX, countStates);
	int k = 0;

	output << typeAutomate << endl;
	output << countX << endl;
	output << countY << endl;
	output << acceptPosition.size() << endl;

	for (unsigned int i = 0; i < newTransposeMatrix.size(); i++)
	{
		for (unsigned int j = 0; j < newTransposeMatrix[i].size(); j++)
			if (find(acceptPosition.begin(), acceptPosition.end(), j) != acceptPosition.end()) {
				output << newTransposeMatrix[i][acceptPosition[k]] << " " << matrix[acceptPosition[k]][i].second << " ";
				k++;
			}
		k = 0;
		output << endl;
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

	int typeAutomate = 2, countStates = 0, countY = 0, countX = 0;
	input >> typeAutomate;
	input >> countX;
	input >> countY;
	input >> countStates;

	vector<vector<pair<int, int>>> matrix = getMatrix(input, countStates, countX);
	vector<vector<int>> allSignals, allClasses;

	setStartMatrixSignals(matrix, allSignals, allClasses);

	vector<vector<int>> newMatrix(countStates, vector<int>(countX));
	vector<int> acceptPosition;
	int countClasses = 0;
	int stop = 0;

	while (true) // пока кол-во состояний не будет равно предыдущему
	{
		acceptPosition.clear();

		setNewMatrix(matrix, allClasses, newMatrix);

		allSignals.clear();

		setMatixSignals(newMatrix, allSignals, allClasses, acceptPosition);

		if (countClasses == allClasses.size()) break; // выход

		countClasses = allClasses.size();
	}

	printMatrix(matrix, newMatrix, acceptPosition, typeAutomate, countX, countY, countStates, output);

	return 0;
}