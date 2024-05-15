# Image Processing

Image Process with C# WebApi with C++ OpenCV module

## Goal of the project

Send image to a webapi and make some operations with opencv package.

## Project Architecture

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/17a30514-72c7-4142-b589-abdd0b8c49df)

This is like an N-tier or Clean Architecture style architecture, because the layers are separated with a light interface connection through the layers.

There are three layers:
  - WebApi
  - Application in C#
  - Application in C++

I kept the WebApi as thin as possible from the endpoint perspective. It is just an interface where the client can send their requests.
Application in C# basically just prepare and validate the image before it sends to the C++ application. Then, the C++ part does the parallel image processing.

## Functionalities

The application receive an image in .png or .jpg format and make a Gaussian Blur method on it and then send it back.

## Logging

To ensure the functionalities I added Serilog and Opentelemetry to make dashboards and see the metrics. You can see the setup in the WebApi layer.

To make dashboard I used the new Aspire standalone application. The setup was quite easy I ran the following command:
 ```docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6 ```

Then to get the token go to:
docker->logs->Login to the dashboard at http://0.0.0.0:18888/login?t=yourToken

More about here: https://www.youtube.com/watch?v=A2pKhNQoQUU&t=400s

Some pictures about the metrics:

Traces:

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/9a6d8e1e-9477-404d-bfcc-0555ac1822e4)

You can see here the duration of a request and several api request that I made.

Request Duration:

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/d915f123-a7aa-448b-a2ad-efb8953ab843)

And many more...

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

At the moment this project only works on Windows!

Setup WebApi as startup project and for x64.

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/1a6ec020-991e-41d3-93ee-42219d735bd5)

### Debug Mode

To setup C++ project in debug mode follow the configuration below (right click on Application.CPP -> Properties):

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/3a47d53f-f6a5-40e6-909e-672b5bd7a585)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/c19f9d0b-1b2c-425b-9dbd-1f892dc6e908)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/66397d52-5bf9-4dad-b031-f109bd51d152)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/793c95cc-d51b-41df-84be-d459c5a79d2a)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/1249c591-71c4-4de5-a696-3f5dd3f94bac)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/a70ec2eb-567e-416e-b1b6-e344db25aae1)

### Release Mode

To setup C++ project in release mode follow the configuration below (right click on Application.CPP -> Properties):

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/d90bb65c-9bf0-4738-8078-5846bf71e8dd)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/8e78be34-8cb3-4d8b-8b49-f789dfc72646)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/40ce6f67-5721-47f6-8ff7-0d37f10d4bae)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/715b74d4-09a7-428e-a64c-59537ec55e79)

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/974a85cf-9335-486d-986a-f2a087d2ef8a)

Now your project should run:

Some Postman example:

Valid Image:

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/61a03c71-c55b-4134-88ec-a7afcc9ac25c)

Invalid Image:

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/076b3aad-d2de-45df-8b20-f7a225ddf318)

## Running the tests

Running tests is only available via Test Explorer

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/ddd48c3c-aff2-4171-ba99-17f6d6e43d03)

## Packages

The project uses quite some packeges:

![image](https://github.com/rieszpeti/Image_Processing/assets/40406762/de86bf6c-f928-4484-9e65-3f42e4be0eb5)

## Further improvements

- add Docker support
- implement REPR pattern to avoid exceptions and have more explanation about errors
- using Fluent Validator for better validation abstraction
- add background jobs or background worker for heavy calculations
- add C++/CLI CLR wrapper
- add linux support, because currently this only works on Windows
- Add more tests

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
