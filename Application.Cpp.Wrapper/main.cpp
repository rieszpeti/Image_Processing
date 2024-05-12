#include "../Application.CPP/main.cpp" // Include the main.cpp header (This is generally not recommended, but I'll follow your request)


// Forward declaration of NativeClass
class NativeClass;

// NativeClassWrapper declaration
public ref class NativeClassWrapper {
private:
    NativeClass* m_nativeClass;

public:
    // Constructor
    NativeClassWrapper() { m_nativeClass = new NativeClass(); }

    // Destructor
    ~NativeClassWrapper() { this->!NativeClassWrapper(); }

    // Finalizer
    !NativeClassWrapper() { delete m_nativeClass; }

    // ImageProcess method to wrap ProcessImageWrapper
    unsigned char* ImageProcess(
        unsigned char* img_pointer,
        long data_len,
        unsigned char* out_result,
        int* length_of_out_result) {

        // Call the wrapped function in NativeClass
        return m_nativeClass->ProcessImageWrapper(img_pointer, data_len, out_result, length_of_out_result);
    }
};