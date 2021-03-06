#include "pch.h"
#include "iostream"
#include "fstream"
#include "string"
#include "vector"
#include "sstream"
#include "algorithm"

using namespace std;

void CreateDotFile(vector<vector<pair<int, int>>>& matrix)
{
	using Edge = std::pair<int, int>;
	using Graph = boost::adjacency_list<boost::vecS,
		boost::vecS, boost::directedS,
		boost::property<boost::vertex_color_t,
		boost::default_color_type>,
		boost::property<boost::edge_weight_t, std::string>>;

	std::vector<Edge> edges;
	std::vector<std::string> weights(edges.size());
	const int VERTEX_COUNT = matrix[0].size();
	for (size_t i = 0; i < matrix[0].size(); ++i)
	{
		for (size_t j = 0; j < matrix.size(); ++j)
		{
			Edge edge(i, matrix[j][i].first);
			edges.push_back(edge);
			weights.push_back(std::to_string(j) + "/" + std::to_string(matrix[j][i].second));
		}
	}

	Graph graph(edges.begin(), edges.end(), weights.begin(),
		VERTEX_COUNT);

	boost::dynamic_properties dp;
	dp.property("weight", boost::get(boost::edge_weight, graph));
	dp.property("label", boost::get(boost::edge_weight, graph));
	dp.property("node_id", boost::get(boost::vertex_index, graph));
	std::ofstream ofs("test.dot");
	boost::write_graphviz_dp(ofs, graph, dp);
}

vector<vector<pair<int, int>>> getMatrixMili(
	ifstream& input,
	int countQ,
	int countX
) // считываем матрицу для мили
{
	vector<vector<pair<int, int>>> matrix(countQ, vector<pair<int, int>>(countX));
	int col = 0, row = 0, state = 0, signal = 0;

	while (input >> state >> signal)
	{
		matrix[row][col] = make_pair(state, signal);
		row++;

		if (row % countQ == 0)
		{
			col++;
			row = 0;
		}
	}

	return matrix;
}

vector<vector<pair<int, int>>> getMatrixMyr(
	ifstream& input,
	int countQ,
	int countX
) // считываем матрицу для мура
{
	vector<int> signals;
	int value = 0;

	for (int i = 0; i < countQ; i++) {
		input >> value;
		signals.push_back(value);
	}

	vector<vector<pair<int, int>>> matrix(countQ, vector<pair<int, int>>(countX));
	value = 0;

	for (int i = 0; i < countX; i++) {
		for (int j = 0; j < countQ; j++) {
			input >> value;
			matrix[j][i] = make_pair(value, signals[j]);
		}
	}

	return matrix;
}

void setStartMatrixSignals(
	vector<vector<pair<int, int>>>& matrix,
	vector<vector<int>>& allClasses
) // отбираем все сигналы у начальной матрицы, и формируем классы эквивалентности
{
	vector<int> temp, tempPos;
	vector<vector<int>> unicSignals;

	for (unsigned int i = 0; i < matrix.size(); i++)
	{
		temp.clear();

		for (unsigned int j = 0; j < matrix[i].size(); j++)
		{
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

vector<vector<int>> transpose(
	const vector<vector<int>>& matrix,
	int countX,
	int countQ
) 
{
	vector<vector<int>> newMatrix(countX, vector<int>(countQ));

	for (unsigned int i = 0; i < matrix.size(); i++)
		for (unsigned int j = 0; j < matrix[i].size(); j++)
			newMatrix[j][i] = matrix[i][j];

	return newMatrix;
}

vector<vector<pair<int, int>>> printMili(
	vector<vector<pair<int, int>>>& matrix,
	vector<vector<int>>& newMatrix,
	vector<int>& acceptPosition,
	int typeAutomate,
	int countX,
	int countY,
	int countQ,
	ofstream& output
)
{
	vector<vector<int>> newTransposeMatrix = transpose(newMatrix, countX, countQ);
	vector<vector<pair<int, int>>> automate;
	vector<pair<int, int>> lineMatrix;
	int acceptIndex = 0;

	output << typeAutomate << endl;
	output << countX << endl;
	output << countY << endl;
	output << acceptPosition.size() << endl;

	for (unsigned int i = 0; i < newTransposeMatrix.size(); i++)
	{
		for (unsigned int j = 0; j < newTransposeMatrix[i].size(); j++)
			if (find(acceptPosition.begin(), acceptPosition.end(), j) != acceptPosition.end())
			{
				output << newTransposeMatrix[i][acceptPosition[acceptIndex]] << " " << matrix[acceptPosition[acceptIndex]][i].second << " ";
				lineMatrix.emplace_back(newTransposeMatrix[i][acceptPosition[acceptIndex]], matrix[acceptPosition[acceptIndex]][i].second);
				acceptIndex++;
			}
		automate.push_back(lineMatrix);
		lineMatrix.clear();
		acceptIndex = 0;
		output << endl;
	}

	return automate;
}

vector<vector<pair<int, int>>> printMyr(
	vector<vector<pair<int, int>>>& matrix,
	vector<vector<int>>& newMatrix,
	vector<int>& acceptPosition,
	int typeAutomate,
	int countX,
	int countY,
	int countQ,
	ofstream& output
) 
{
	vector<vector<int>> newTransposeMatrix = transpose(newMatrix, countX, countQ);
	vector<vector<pair<int, int>>> automate;
	vector<pair<int, int>> lineMatrix;
	int acceptIndex = 0;

	output << typeAutomate << endl;
	output << countX << endl;
	output << countY << endl;
	output << acceptPosition.size() << endl;

	for (auto item : acceptPosition)
		output << matrix[item][0].second << " ";

	output << endl;

	for (unsigned int i = 0; i < newTransposeMatrix.size(); i++)
	{
		for (unsigned int j = 0; j < newTransposeMatrix[i].size(); j++)
		{
			if (find(acceptPosition.begin(), acceptPosition.end(), j) != acceptPosition.end()) {
				output << newTransposeMatrix[i][acceptPosition[acceptIndex]] << " ";
				lineMatrix.emplace_back(newTransposeMatrix[i][acceptPosition[acceptIndex]], matrix[acceptPosition[acceptIndex]][i].second);
				acceptIndex++;
			}
		}
		automate.push_back(lineMatrix);
		lineMatrix.clear();
		acceptIndex = 0;
		output << endl;
	}

	return automate;
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

	int typeAutomate = 2, countQ = 0, countY = 0, countX = 0, countClasses = 0;

	input >> typeAutomate;
	input >> countX;
	input >> countY;
	input >> countQ;

	vector<vector<pair<int, int>>> matrix;

	if (typeAutomate == 1)
		matrix = getMatrixMyr(input, countQ, countX);
	else if (typeAutomate == 2)
		matrix = getMatrixMili(input, countQ, countX);

	vector<vector<int>> allClasses;
	vector<vector<int>> newMatrix(countQ, vector<int>(countX));
	vector<int> acceptPosition;

	setStartMatrixSignals(matrix, allClasses);

	while (true) // пока кол-во состояний не будет равно предыдущему
	{
		acceptPosition.clear();
		setNewMatrix(matrix, allClasses, newMatrix);
		setMatixSignals(newMatrix, allClasses, acceptPosition);

		if (countClasses == allClasses.size()) break; // выход

		countClasses = allClasses.size();
	}

	vector<vector<pair<int, int>>> automate;

	if (typeAutomate == 1) {
		automate = printMyr(matrix, newMatrix, acceptPosition, typeAutomate, countX, countY, countQ, output);
		CreateDotFile(automate);
	}
	else if (typeAutomate == 2) {
		automate = printMili(matrix, newMatrix, acceptPosition, typeAutomate, countX, countY, countQ, output);
		CreateDotFile(automate);
	}
		
	return 0;
}