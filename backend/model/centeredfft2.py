import numpy as np

def centeredfft2(Z, FsX, FsY):
    M = np.size(Z, 1)
    N = np.size(Z, 0)

    k = range(-M // 2, M // 2)
    freqX = k / (M / FsX)

    k = range(-N // 2, N // 2)
    freqY = k / (N / FsY)

    fft = np.fft.fft2(Z) / (M * N)
    fft2 = (1 / (FsY * FsX)) * np.fft.fft2(Z)
    fft = np.fft.fftshift(fft)
    fft2 = np.fft.fftshift(fft2)

    return (freqX, freqY, fft, fft2)
