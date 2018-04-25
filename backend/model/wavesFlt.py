from .centeredfft2 import centeredfft2
import numpy as np

def wavesFlt(X, Y, height, lambdaC=0.08):
    fc = 1 / lambdaC # The Roughness Long-wavelength Cutoff frequency

    # Computing the spatial frequency spectrum
    FsX = 1/(X(2) - X(1)) # The sampling frequency in the X direction
    FsY = 1/(Y(2) - Y(1)) # The sampling frequency in the Y direction
    freqX, freqY, FFT, _ = centeredfft2(height, FsX, FsY)

    # Creating the filter
    alpha = np.sqrt(np.log(2) / np.pi)

    filt_wavi = np.zeros(len(freqX))
    for i in range(len(freqX)):
        filt_wavi[:, i] = 1 - np.exp(np.pi * (-(alpha*freqX[i]/fc) ** 2 - (alpha * freqY / fc) ** 2))

    M = len(X)
    N = len(Y)
    MN = M * N

    waviFFT = np.abs(filt_wavi) * np.abs(FFT)  * MN # compensating for the centeredFFT normalization
    waviFFT2 = np.abs(filt_wavi) * np.abs(FFT) * FsX * FsY # Compensating for the centeredFFT normalization
    abc = np.fft.ifftshift(waviFFT)
    abcd = np.fft.ifftshift(waviFFT2) # Inverse shifting so as to get into the form recognized by matlab.
    waviHeights = np.fft.irfft2(abc) # 'symmetric' flag ensures real output, so use irfft2
    waviHeights2 = np.fft.irfft2(abcd)

    # Subtracting the waviHeights from the actual surface to get the roughness Roughness heights.
    return (waviHeights, waviHeights2)
