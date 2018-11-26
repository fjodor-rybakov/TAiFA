#include "pch.h"
#include <iostream>
#include "string"
#include <fstream>
#include <sstream>
#include "vector"
#include <algorithm>
#include "queue"

using namespace std;

void CreateDotFile(vector<vector<vector<int>>>& matrix)
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
	for (size_t i = 0; i < matrix.size(); ++i)
	{
		for (size_t j = 0; j < matrix[i].size(); ++j)
		{
			if (matrix[i][j][0] != -1) {
				Edge edge(i, matrix[i][j][0]);
				edges.push_back(edge);
				weights.push_back(std::to_string(j));
			}
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
		cout << str << endl;
	}

	return matrix;
}

bool isNotExistElement(vector<int> newVector, int value) {
	return find(newVector.begin(), newVector.end(), value) == newVector.end();
}

bool searchValue(vector<vector<vector<int>>> matrix, int p) {
	for (auto &i : matrix)
		for (auto &j : i)
			if (j[0] == p)
				return true;
	return false;
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

void restoreElem(vector<vector<vector<int>>>& matrix) {
	for (unsigned int i = 0; i < matrix.size(); i++)
		for (unsigned int j = 0; j < matrix[i].size(); j++) 
			if (matrix.size() < matrix[i][j][0] && matrix[i][j][0] != -1) {
				matrix[i][j][0] -= matrix[i][j][0] - matrix.size() + 1;
			}
}

void filter(vector<vector<vector<int>>>& matrix) {
	for (unsigned int p = 1; p < matrix.size(); p++)
		if (!searchValue(matrix, p)) {
			//matrix[p].clear();
			matrix[p] = matrix.back();
			matrix.pop_back();
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

void restoreOldValues(vector<vector<vector<int>>>& tempMatrix, vector<vector<vector<int>>>& matrix) {
	for (int i = 0; i < tempMatrix.size(); i++) {
		for (int j = 0; j < tempMatrix[i].size(); j++) {
			if (tempMatrix[i][j].size() != 1) {
				for (int a = 0; a < matrix.size(); a++) {
					for (int s = 0; s < matrix[a].size(); s++) {
						if (matrix[a][s] == tempMatrix[i][j]) {
							tempMatrix[i][j].clear();
							tempMatrix[i][j].push_back(tempMatrix[a][s][0]);
						}
					}
				}
			}
		}
	}
}

vector<vector<vector<int>>> getDeterminationMatrix(const vector<vector<vector<int>>>& originMatrix, const int& x) {
	vector<vector<vector<int>>> matrix, test;
	vector<vector<int>> temp, oldStates;
	vector<int> value;
	queue<vector<int>> queue;
	int state = 0, index = 0;

	value.push_back(0);
	queue.push(value);

	while (!queue.empty()) {
		value = queue.front();

		if (value.size() != 1) {
			temp.clear();
			temp.assign(originMatrix[value[0]].begin(), originMatrix[value[0]].end());

			for (unsigned int i = 1; i < value.size(); i++)
				merge(temp, originMatrix[value[i]]);

			for (auto i : temp)
				if (find(oldStates.begin(), oldStates.end(), value) == oldStates.end()) {
					queue.push(i);
					oldStates.push_back(i);
				}

			if (find(matrix.begin(), matrix.end(), temp) == matrix.end()) {
				matrix.push_back(temp);
				test.push_back(temp);
				for (int i = 0; i < x; i++) {
					if (matrix[matrix.size() - 1][i].size() != 1) {
						test[test.size() - 1][i].clear();
						//cout << state << endl;
						test[test.size() - 1][i].push_back(matrix[matrix.size() - 1][i].size());
					}
				}
			}
		}
		else 
		{
			if (find(matrix.begin(), matrix.end(), originMatrix[value[0]]) == matrix.end()) {
				matrix.push_back(originMatrix[value[0]]);
				test.push_back(matrix[value[0]]);
				for (int i = 0; i < x; i++) {
					if (matrix[matrix.size() - 1][i].size() != 1) {
						test[test.size() - 1][i].clear();
						//cout << state << endl;
						test[test.size() - 1][i].push_back(matrix[matrix.size() - 1][i].size());
					}
				}
			}

			for (unsigned int i = 0; i < x; i++) {
				if (originMatrix[value[0]][i][0] != -1) {
					queue.push(originMatrix[value[0]][i]);
				}
			}
		}

		/*if (state == 3) {
			break;
		}*/

		queue.pop();
		state++;
	}
	
	cout << endl;
	printMatrix(matrix);
	cout << endl;
	printMatrix(test);

	return test;
}

/*vector<vector<vector<int>>> getDeterminationMatrix(vector<vector<vector<int>>>& matrix, unsigned int& counter) {
	vector<vector<vector<int>>> tempMatrix = matrix;
	int value = 0;
	vector<vector<int>> temp;
	vector<vector<vector<int>>> oldStates;
	for (unsigned int i = 0; i < matrix.size(); i++) {
		for (unsigned int j = 0; j < matrix[i].size(); j++) {
			if (matrix[i][j].size() != 1) {
				temp.clear();
				temp.assign(matrix[matrix[i][j][0]].begin(), matrix[matrix[i][j][0]].end());
				for (unsigned int k = 1; k < matrix[i][j].size(); k++) {
					value = matrix[i][j][k];
					merge(temp, matrix[value]);
				}
				if (find(oldStates.begin(), oldStates.end(), temp) == oldStates.end()) {
					oldStates.push_back(temp);
					matrix.push_back(temp);
					tempMatrix.push_back(temp);
					tempMatrix[i][j].clear();
					tempMatrix[i][j].push_back(tempMatrix.size() - 1);
				}
			}
		}
	}

	//restoreOldValues(tempMatrix, matrix);
	filter(tempMatrix);

	return tempMatrix;
}*/

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
	result = getDeterminationMatrix(matrix, x);
	//printMatrix(result);
	//cout << endl;
	//restoreElem(result);
	//printMatrix(result);
	CreateDotFile(result);
}