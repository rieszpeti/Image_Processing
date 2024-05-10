//main.cpp

#include <string>
#include <vector>
#include <iostream>
#include <opencv2/core.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <stdio.h>
#include "main.h"

using namespace cv;
using namespace std;

#define EXPORTED_METHOD extern "C" __declspec(dllexport)

EXPORTED_METHOD
void GetGaussImg(char* buffer) {};

EXPORTED_METHOD
void ImageProcessCPP(const char* str)
{
	std::string base64String(str);

	std::vector<uchar> data(base64String.begin(), base64String.end());
	cv::Mat img = cv::imdecode(data, cv::ImreadModes::IMREAD_COLOR);

	cv::Mat grayImage;
	cv::cvtColor(img, grayImage, cv::COLOR_BGR2GRAY);

	cv::Mat blurredImage;
	cv::GaussianBlur(grayImage, blurredImage, cv::Size(5, 5), 0, 0);

	std::vector<uchar> imageData;
	cv::imencode(".jpg", blurredImage, imageData);

	std::string base64Image(imageData.begin(), imageData.end());

	const char* charArr = base64Image.c_str();

	GetGaussImg((char*)charArr);

	//return base64Image;
}

void processROI(cv::Mat& image, const cv::Rect& roi) {
    cv::Mat roiImage = image(roi);
    cv::GaussianBlur(roiImage, roiImage, cv::Size(5, 5), 0, 0);
}

void ImageProcessCPP()//const std::string& imagePath)
{
    cv::Mat img = cv::imread("C:/Users/SillySharp/Desktop/img.png");

    cv::imshow("Original Image", img);
    cv::waitKey(0);

    unsigned int numThreads = std::thread::hardware_concurrency();

    // Define ROIs
    std::vector<cv::Rect> rois;
    int rowsPerThread = img.rows / numThreads;
    int colsPerThread = img.cols / numThreads;

    for (int r = 0; r < img.rows; r += rowsPerThread) {
        for (int c = 0; c < img.cols; c += colsPerThread) {
            cv::Rect roi(c, r, colsPerThread, rowsPerThread);
            rois.push_back(roi);
        }
    }

    // Process ROIs using multiple threads
    std::vector<std::thread> threads;
    for (const auto& roi : rois) {
        threads.emplace_back(processROI, std::ref(img), roi);
    }

    // Wait for all threads to finish
    for (auto& thread : threads) {
        thread.join();
    }

    // Display the processed image
    cv::imshow("Processed Image", img);
    cv::waitKey(0);
}

int main()
{
	std::string base64String = "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";
    ImageProcessCPP();
	return 0;
}


EXPORTED_METHOD
int DoSomethingInC(unsigned short int ExampleParam, unsigned char AnotherExampleParam) {
    printf("You called method DoSomethingInC(), You passed in %d and %c\n", ExampleParam, AnotherExampleParam);
	return 42;
}


//// Load the image
//cv::Mat img = cv::imread(imagePath);
//if (img.empty())
//{
//    std::cerr << "Failed to load image: " << imagePath << std::endl;
//    return;
//}


//cv::Rect roi(100, 100, 200, 200);

//cv::Mat image_roi = img(roi);
////cv::Mat image_roi2;

//cv::Mat blurredImage;
//cv::GaussianBlur(img, blurredImage, cv::Size(5, 5), 0, 0);
//blurredImage.copyTo(image_roi);

// Apply GaussianBlur using multiple threads
//int numThreads = cv::getNumThreads();
//cv::setNumThreads(std::min(numThreads, 16)); // Utilize at most 16 threads
//cv::Mat blurredImage;
//cv::GaussianBlur(img, blurredImage, cv::Size(5, 5), 0, 0);

// Display the blurred image
//cv::imshow("Blurred Image", blurredImage);
//cv::waitKey(0);