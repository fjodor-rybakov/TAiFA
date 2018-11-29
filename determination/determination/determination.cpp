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

void CreateDotFile(vector<vector<vector<int>>>& matrix)
{
	/*using Edge = std::pair<int, int>;
	using Graph = boost::adjacency_list<boost::vecS,
		boost::vecS, boost::directedS,
		boost::property<boost::vertex_color_t,
		boost::default_color_type>,
		boost::property<boost::edge_weight_t, std::string>>;

	std::vector<Edge> edges;
	std::vector<std::string> weights(edges.size());
	const int VERTEX_COUNT = matrix.size() + 1;
	for (size_t i = 0; i < matrix.size(); i++)
	{
		for (size_t j = 0; j < matrix[i].size(); j++)
		{
			if (matrix[i][j][0] != -1) {
				Edge edge(i, matrix[i][j][0]);
				edges.push_back(edge);
				weights.push_back(std::to_string(j));
			}
		}
	}

	Graph graph(edges.begin(), edges.end(), weights.begin(), VERTEX_COUNT);

	boost::dynamic_properties dp;
	dp.property("weight", boost::get(boost::edge_weight, graph));
	dp.property("label", boost::get(boost::edge_weight, graph));
	dp.property("node_id", boost::get(boost::vertex_index, graph));
	std::ofstream ofs("test.dot");
	boost::write_graphviz_dp(ofs, graph, dp);*/

	ofstream dotFile("test.dot");

}

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

vector<vector<vector<int>>> setMatrix(ifstream& input, const unsigned int z, const unsigned int x) {
	vector<vector<vector<int>>> matrix(z, vector<vector<int>>(x, vector<int>(1, -1)));
	string str;
	int toState = 0, signal = 0, col = 0;
	while (getline(input, str)) {
		stringstream ss(str);

		while (ss >> toState >> signal) {
			if (matrix[col][signal][0] == -1)
				matrix[col][signal].clear();
			matrix[col][signal].push_back(toState);
		}

		col++;
	}

	return matrix;
}

bool isNotExistElement(vector<int> newVector, int value) {
	return find(newVector.begin(), newVector.end(), value) == newVector.end();
}

void printMatrix(const vector<vector<vector<int>>>& matrix) {
	for (auto &i : matrix) {
		for (auto &j : i) {
			for (int k : j) {
				cout << k << " ";
			}
			cout << "| ";
		}
		cout << endl;
	}
}

void merge(vector<vector<int>>& temp, vector<vector<int>> newVector) {
	for (unsigned int i = 0; i < temp.size(); i++) {
		for (unsigned int j = 0; j < newVector[i].size(); j++) {
			if (newVector[i][j] != -1 && isNotExistElement(temp[i], newVector[i][j])) {
				temp[i].push_back(newVector[i][j]);
				sort(temp[i].begin(), temp[i].end());
			}
		}
	}
}

void restoreMatrix(vector<vector<vector<int>>>& matrix, map<int, vector<int>>& indexStates) {
	for (unsigned int i = 0; i < matrix.size(); i++) {
		for (unsigned int j = 0; j < matrix[0].size(); j++) {
			for (auto& item : indexStates) {
				if (item.second == matrix[i][j]) {
					matrix[i][j].clear();
					matrix[i][j].push_back(item.first);
				}
			}
		}
	}
}

void printOutput(ofstream& output, vector<vector<vector<int>>>& matrix, vector<int>& finishStates, int x) {
	output << x << endl;
	output << matrix.size() << endl;
	output << finishStates.size() << endl;
	for (auto& i : finishStates) {
		output << i << " ";
	}
	output << endl;
	for (auto &i : matrix) {
		for (auto &j : i) {
			for (int k : j) {
				output << k << " ";
			}
			output << " ";
		}
		output << endl;
	}
}

vector<vector<vector<int>>> getDeterminationMatrix(
	vector<vector<vector<int>>>& originMatrix, 
	const int& x, 
	vector<int>& finishStates
) {
	for (int i = 0; i < originMatrix.size(); i++) {
		for (int j = 0; j < originMatrix[i].size(); j++) {
			sort(originMatrix[i][j].begin(), originMatrix[i][j].end());
		}
	}
	vector<vector<vector<int>>> matrix, result;
	set<vector<int>> oldStates;
	vector<vector<int>> temp;
	set<int> newFinishStates;
	vector<int> value;
	queue<vector<int>> queue;
	map<int, vector<int>> indexStates;

	value.push_back(0);
	queue.push(value);

	while (!queue.empty()) {
		value = queue.front();

		if (find(oldStates.begin(), oldStates.end(), value) == oldStates.end()) {
			if (find(matrix.begin(), matrix.end(), originMatrix[value[0]]) == matrix.end())
				matrix.push_back(originMatrix[value[0]]);

			if (find(finishStates.begin(), finishStates.end(), value[0]) != finishStates.end()) {
				newFinishStates.insert(value[0]);
			}

			if (value.size() == 1) {
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

				for (auto item : temp)
					if (item[0] != -1) 
						queue.push(item);

				matrix.push_back(temp);
				indexStates[matrix.size() - 1] = value;
				for (auto item : finishStates) {
					if (find(value.begin(), value.end(), item) != value.end()) {
						newFinishStates.insert(matrix.size() - 1);
					}
				}
			}
		}

		oldStates.insert(value);
		queue.pop();
	}

	finishStates.clear();
	finishStates.assign(newFinishStates.begin(), newFinishStates.end());
	restoreMatrix(matrix, indexStates);
	printMatrix(matrix);

	return matrix;
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

	unsigned int x = 0, z = 0, k = 0, counter = 0;
	vector<int> finishStates;

	input >> x;
	input >> z;
	input >> k;
	finishStates = setFinishStates(input);
	counter = z;

	vector<vector<vector<int>>> matrix = setMatrix(input, z, x), result;
	result = getDeterminationMatrix(matrix, x, finishStates);
	printOutput(output, result, finishStates, x);
	//CreateDotFile(result);
}