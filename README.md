# The Goal:

This project is meant to showcase different ways to capture a Unity camera with a shader into several RenderTextures. It also shows different ways to convert the captured textures into Unitys Texture2D format, which can be saved or applied to objects. The final result of each solution should be the same, thus we are mainly looking into the FPS impact of each solution. For the purpose of testing a method called Blit(), I made use of a simple depth shader.

## Capturing camera into RenderTextures:

There are 2 main ways to capture view with different shaders from the same viewing angle into a RenderTexture. The first and simpelest way to do it is by creating a new camera with the same transform, and giving this camera a replacement shader. The second way is to attach a script that uses the Graphics.Blit call to capture the camera view into multiple RenderTextures. This method has an optional parameter for a shader which can be used instead of a replacement shader. 

### Impact of image size:
After implementing the functionallity and gathering the average FPS under the differrent conditions, and organizing the data in the chart below, a few things become apperent. The first thing you can notice is that the performance does not significantly differ between output image sizes if we make use of 1 camera and the Graphics.Blit call. Another thing you can notice is how that if we use 2 cameras to capture our 2 RenderTextures, the performance is roughly equel to the single camera if we use small images, but has much less fps in bigger images.

![Comparing blit call image size](https://user-images.githubusercontent.com/60891244/233868953-ea95ad76-c3f4-4c8b-ac0e-25e80c6732f5.png)

### Impact of output amount:
It can also be important to see how both of these solutions scale if we want to capture the view with many differrent shaders instead of the 2 we tested so far. To see this impact, I conducted a test which outputs the same image into 5 seperate RenderTextures and an original texture without any shader. If we look at the image below, we can see that having a seperate camera for each texture is once again slower than if we make use of a single Blit call.

![Comparing blit call image amount](https://user-images.githubusercontent.com/60891244/233868955-16d757d1-5128-4f07-b51c-d47831c83b80.png)

### Conclusion:
With the test results above, I can conclude that the Graphics.Blit call has significantly better performance than having several cameras with replacement shaders. One thing were multiple cameras can be more usefull is if we want to have slightly different camera settings or positions.

## Converting RenderTextures into Texture2Ds:
Sometimes it can be necessary to convert a RenderTexture into a Texture2D. For example when one wants to save the image. Below I analyze a way using the ReadPixels method and the AsyncGPUReadback method. Because the second method is asynchronous, it also made a synchronous implementation of the method. There are also different methods to copy image data like the Graphics.CopyTexture method, but this is not suitable as this only copies the data on the GPU, while we want our data on the CPU.

### Comparing different image sizes:
If we look at the figure below, we can see the performance of the 3 implementations under 2 different iamge sizes. The AsyncGPUReadback executing asnchronously is the fastest with both image sizes. The same method running synchronously comes in second, while the ReadPixels method comes in last. Another thing you can notice is how the difference in performance becomes smaller as the image sizes increase.

![Different conversion methods to Texture2D](https://user-images.githubusercontent.com/60891244/233868844-ca72edf9-a17a-4f61-b920-9f30ffe2be9e.png)


### Conclusion:
Though the ASyncGPUReadback is the fastest of the three implementations, due to it being asynchronous, it might be more unreliable, and not as user friendly for people without experience programmign asynchronously. With the difference in performance becoming smaller the bigger the output images are, It can be the case that the performance is slighlty worse if the images get too big.

https://imgur.com/a/LJiJeZt
