#include <string>
#include <vector>
#include <iostream>
#include <opencv2/core.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <stdio.h>

using namespace cv;
using namespace std;

#define EXPORTED_METHOD extern "C" __declspec(dllexport)

struct ImageInfo
{
	unsigned char* data;
	int size;
};

EXPORTED_METHOD
bool ReleaseMemoryFromC(unsigned char* buf)
{
	if (buf == NULL)
	{
		return false;
	}

	free(buf);
	return true;
}

EXPORTED_METHOD
void ProcessImageCpp(
	unsigned char* img_pointer,
	long data_len,
	const char* file_extension,
	ImageInfo& imInfo)
{
	vector<unsigned char> inputImageBytes(img_pointer, img_pointer + data_len);

	cv::Mat img = imdecode(inputImageBytes, 1);

	int kernel_size = 5;
	int sigma_x = 5;
	int sigma_y = 5;

	// Return value
	// Number of concurrent threads supported.
	// If the value is not well defined or not computable, returns ​0​.
	unsigned int numOfConcurrentThreads = std::thread::hardware_concurrency();

	if (numOfConcurrentThreads == 0)
	{
		numOfConcurrentThreads = 1;
	}

	// Apply Gaussian along rows in parallel
	cv::parallel_for_(cv::Range(0, img.rows), [&](const cv::Range& range) 
	{
		for (int r = range.start; r < range.end; r++) {
			cv::Mat inRow = img.row(r);
			cv::GaussianBlur(inRow, inRow, cv::Size(kernel_size, 1), sigma_x, 0);
		}
	}, numOfConcurrentThreads);

	//// Apply Gaussian along columns in parallel
	//cv::parallel_for_(cv::Range(0, img.cols), [&](const cv::Range& range) 
	//{
	//	for (int c = range.start; c < range.end; c++) {
	//		cv::Mat col = img.col(c);
	//		cv::GaussianBlur(col, col, cv::Size(1, kernel_size), 0, sigma_y);
	//	}
	//});

	cv::imwrite("C:/Users/SillySharp/Desktop/out.png", img);

	vector<unsigned char> bytes;
	cv::imencode(file_extension, img, bytes);

	imInfo.size = bytes.size();
	imInfo.data = (unsigned char*)calloc(imInfo.size, sizeof(unsigned char));
	std::copy(bytes.begin(), bytes.end(), imInfo.data);
}