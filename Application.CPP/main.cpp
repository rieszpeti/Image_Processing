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

void ProcessROI(cv::Mat& image, const cv::Rect& roi) {
    cv::Mat roiImage = image(roi);
    cv::GaussianBlur(roiImage, roiImage, cv::Size(5, 5), 0, 0);
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
    cv::Mat processed;

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
        threads.emplace_back(ProcessROI, std::ref(img), roi);
    }

    // Wait for all threads to finish
    for (auto& thread : threads) {
        thread.join();
    }

    imwrite("C:/Users/SillySharp/Desktop/output.png", img);

    vector<unsigned char> bytes;
    imencode(file_extension, img, bytes);

    imInfo.size = bytes.size();
    imInfo.data = (unsigned char*)calloc(imInfo.size, sizeof(unsigned char));
    std::copy(bytes.begin(), bytes.end(), imInfo.data);
}