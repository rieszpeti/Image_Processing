using Application.REPR;

namespace Application.Interfaces
{
    public interface IImageProcessingService
    {
        Task<ImageProcessResponse> ProcessImage(ImageProcessRequest request);
    }
}