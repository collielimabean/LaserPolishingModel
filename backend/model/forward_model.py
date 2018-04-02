from .centeredfft2 import centeredfft2
from .forward_model_config import ForwardModelConfig
from .zygo import ZygoAsciiFile
from .ripples import ripples
import matplotlib.pyplot as plt
import numpy as np

T_0 = 298 # [K], initial temp XXX - make this configurable as

# Linear curve fitting emperical model from 2009 Tyler Perry Paper
# Curve fitting for pulse durations and melt time relationship from Tyler (SS316)
PULSE_DURATION = [0, 50, 100, 200, 300, 400, 500, 750, 1000]       # [ns] Pulse duration
MAX_MELT_TIME = [0, 102, 215, 406, 653, 868, 1096, 1615, 2090]     # [ns] Maximum melt time 


def run_forward_model(zygo, material, laser, config=ForwardModelConfig()):
    if config.show_code_settings:
        print(str(config))

    #  MELT TIME CALCULATION - Maximun melt time estimation - four models, set in prediction settings above
    if config.melt_time_assumption == 'Nicholas':
        melt_time = (laser.pulse_duration * 1e9) / laser.duty_cycle
    elif config.melt_time_assumption == 'Justin':
        melt_time = laser.pulse_duration * 1e9 * 12
    elif config.melt_time_assumption == 'Brodan':
        cv_prime = material.rho * (133 + 192.3 / (material.T_b - T_0))
        melt_time = ((material.T_b - material.T_m) ** 2) * \
            (np.pi ** 3) * (laser.beam_radius ** 4) * (cv_prime ** 2) * \
            (material.alpha_td / (16 * (material.absp ** 2) * laser.pulse_average_power)) * 1e9
    else:
        p = np.polyfit(PULSE_DURATION, MAX_MELT_TIME, 1)
        melt_time = p(1) * (laser.pulse_duration * 1e9) + p(2)

    # [K] Maximum melt temperature in center of melt pool using 1D model
    Tn = 4 * (material.absp) * (laser.pulse_average_power / \
        ((np.pi ** 1.5) * (laser.beam_radius ** 2) * material.rho_c) * np.sqrt(laser.pulse_duration / material.alpha_td))

    # dimensionless temperature
    theta_m = (material.T_m - T_0) / Tn

    # [-] Normalized Average Displacement
    ln = -18.51 * material.stc * material.absp \
        * laser.pulse_average_power * (laser.pulse_duration ** 2) \
        / (material.mu * material.rho_c * (laser.beam_radius ** 4)) \
        * (1 - theta_m) * np.exp(-8.80 * theta_m)

    # [-] Average Feature Slope - Actual experimental value for S7 Tool Steel (Nicholas)
    afs = 0.001 * ln

    # CRITICAL FREQUENCY CALCULATION -Calculation of critical frequency for dampening of roughness features
    # From Tyler Perry's 2009 Paper, and Madu's ____ %%% TODO - Figure out specific paper it is from
    # Critical Frequency calculation
    fcr = np.sqrt(material.rho / (8 * (np.pi ** 2) * material.mu * melt_time * 1e-9)) * 1e-3 # [1 / mm] Critical Frequency Calculation
    fwall = 1 / ((2 * laser.beam_radius) * 1e3) # [1 / mm] 'Wall Frequency' - i.e. frequency of diameter of beam

    # If critical frequency is less than wall frequency, use wall frequency
    if fcr < fwall:
        fcr = fwall

    # load surface & accompanying axis vectors
    surface = zygo.phase_data * 1e6
    X = np.arange(0, (np.size(surface, 1)) * surface.camera_resolution)
    Y = np.arange(0, (np.size(surface, 0)) * surface.camera_resolution)

    # TODO: knnimpute

    # TODO: remove least squares mean plane //

    surface = np.flipud(surface)

    # run 2D FFT
    FsX = 1 / zygo.camera_resolution
    FsY = 1 / zygo.camera_resolution

    freqX, freqY, fft, fft2 = centeredfft2(surface, FsX, FsY)
    if config.show_console_output:
        print(fft)
        print(freqX)
        print(freqY)

    # CROSS SECTION OF THE FREQUENCY SPECTRUM - Takes center of FFT matrix
    # USES THE OUTLINE INSTEAD OF PROFILE, WANT TO LOOK INTO BEST METHOD
    fft_ind_x = np.where(freqX == 0)[0]
    fft_x = fft[fft_ind_x]

    fft_ind_y = np.where(freqY == 0)[0]
    fft_y = fft[fft_ind_y]

    # capillary prediction starts here
    # create capillary low pass filter
    if config.show_debugging_figures:
        plt.figure() 
        plt.plot(freqX.T, np.abs(fft_x))
        plt.title('Cross-Section Frequency spectrum in X-direction of Sample')

        plt.figure()
        plt.plot(freqY.T, abs(fft_y))
        plt.title('Cross-Section Frequency spectrum in Y-direction of Sample')

    cap_filter = np.zeros(np.size(fft)).T
    for i in range(len(freqY)):
        cap_filter[:, i] = np.exp(-np.power((freqX / fcr), 2) - np.power(freqY[i] / fcr, 2))
    cap_filter = cap_filter.T

    if config.show_debugging_figures:
        pass
        #plt.figure()
        #plt.surface(freqX_unpo,freqY_unpo,filter,'LineStyle','none')
        #plt.title('Surface plot of filter for X and Y Frequencies')

    # APPLYING CAPILLARY LOW-PASS FILTER ON UNPOLISHED SURFACE
    M = np.size(surface, 0)
    N = np.size(surface, 1)

    redcFFT = cap_filter * fft # Compensating for the centeredFFT normalization
    abc = np.fft.ifftshift(redcFFT) # Inverse shifting so as to get into the form recognized by matlab.
    Z_redc = np.fft.ifft2(abc) * M * N

    # plotting and more

    ### thermocapillary prediction
    Z_rip = ripples(X, Y, afs, 100, 10, 5)

    # another plot

    # COMBINE THERMOCAPILLARY RIPPLES TO CAPILLARY FILTERED SURFACE AND PLOT
    Z_pred = Z_redc + Z_rip

    # 2D FAST FOURIER TRANSFORMATION OF THE PREDICTED SURFACE
    freqX_pred, freqY_pred, FFT_pred, _ = centeredfft2(Z_pred, FsX, FsY)

    if config.show_debugging_figures:
        plt.figure()
        plt.plot(freqX_pred, np.abs(FFT_pred))
        plt.title('Total Frequency spectrum in X-direction of Predicted Sample')

        plt.figure() 
        plt.plot(freqY_pred, np.abs(FFT_pred))
        plt.title('Total Frequency spectrum in Y-direction of Predicted Sample')

    # CROSS SECTION OF THE FREQUENCY SPECTRUM OF THE PREDICTED SURFACE
    fft_pred_ind_x = np.where(freqX_pred == 0)[0]
    fft_pred_x = FFT_pred[fft_pred_ind_x]

    fft_pred_ind_y = np.where(freqY_pred == 0)[0]
    fft_pred_y = FFT_pred[fft_pred_ind_y]

    if config.show_debugging_figures:
        plt.figure() 
        plt.plot(freqX_pred.T, np.abs(fft_pred_x))
        plt.title('Cross-Section Frequency spectrum in X-direction of Predicted Sample')

        plt.figure()
        plt.plot(freqY_pred.T, np.abs(fft_pred_y))
        plt.title('Cross-Section Frequency spectrum in Y-direction of Predicted Sample')


    """
    %% APPLY HIGH-PASS GAUSSIAN FILTERS TO ALL SURFACES TO REMOVE WAVINESS
    % Uses wavesFlt to apply high pass gaussian filter to filter out 80 um wavelengths
    [Z_wavifiltered_unpo] = wavesFlt(X_unpo, Y_unpo, Z_unpo);
    [Z_wavifiltered_po] = wavesFlt(X_po, Y_po, Z_po);
    [Z_wavifiltered_pred] = wavesFlt(X_unpo, Y_unpo, Z_pred);

    %% CALCULATE SURFACE ROUGHNESS 
    Sa_unpo = sum(sum(abs(Z_unpo-mean(mean(Z_unpo)))))./numel(Z_unpo)*1000;
    Sq_unpo = sqrt(sum(sum((Z_unpo-mean(mean(Z_unpo))).^2))./numel(Z_unpo))*1000;

    Sa_po = sum(sum(abs(Z_po-mean(mean(Z_po)))))./numel(Z_po)*1000;
    Sq_po = sqrt(sum(sum((Z_po-mean(mean(Z_po))).^2))./numel(Z_po))*1000;

    Sa_pred = sum(sum(abs(Z_pred-mean(mean(Z_pred)))))./numel(Z_pred)*1000;
    Sq_pred = sqrt(sum(sum((Z_pred-mean(mean(Z_pred))).^2))./numel(Z_pred))*1000;

    Sa_wavifiltered_unpo = sum(sum(abs(Z_wavifiltered_unpo-mean(mean(Z_wavifiltered_unpo)))))./numel(Z_wavifiltered_unpo)*1000;
    Sq_wavifiltered_unpo = sqrt(sum(sum((Z_wavifiltered_unpo-mean(mean(Z_wavifiltered_unpo))).^2))./numel(Z_wavifiltered_unpo))*1000;

    Sa_wavifiltered_po = sum(sum(abs(Z_wavifiltered_po-mean(mean(Z_wavifiltered_po)))))./numel(Z_wavifiltered_po)*1000;
    Sq_wavifiltered_po = sqrt(sum(sum((Z_wavifiltered_po-mean(mean(Z_wavifiltered_po))).^2))./numel(Z_wavifiltered_po))*1000;

    Sa_wavifiltered_pred = sum(sum(abs(Z_wavifiltered_pred-mean(mean(Z_wavifiltered_pred)))))./numel(Z_wavifiltered_pred)*1000;
    Sq_wavifiltered_pred = sqrt(sum(sum((Z_wavifiltered_pred-mean(mean(Z_wavifiltered_pred))).^2))./numel(Z_wavifiltered_pred))*1000;

    fprintf('Sa_unpo, Sa_wavifiltered_unpo, Sq_unpo, Sq_wavifiltered_unpo')
    round([Sa_unpo Sa_wavifiltered_unpo Sq_unpo Sq_wavifiltered_unpo])

    fprintf('Sa_po, Sa_wavifiltered_po, Sq_po, Sq_wavifiltered_po')
    round([Sa_po Sa_wavifiltered_po Sq_po Sq_wavifiltered_po])

    fprintf('Sa_pred, Sa_wavifiltered_pred, Sq_pred, Sq_wavifiltered_pred')
    round([Sa_pred Sa_wavifiltered_pred Sq_pred Sq_wavifiltered_pred])

    %% SUM FREQUENCY SPECTRUM
    % Uses for if calling multiple times - Needs to be incorporated
    FFT_unpo_X_sum = FFT_unpo_X_sum + FFT_unpo_X;
    FFT_unpo_Y_sum = FFT_unpo_Y_sum + FFT_unpo_Y;

    FFT_pred_X_sum = FFT_pred_X_sum + FFT_pred_X;
    FFT_pred_Y_sum = FFT_pred_Y_sum + FFT_pred_Y;

    FFT_po_X_sum = FFT_po_X_sum + FFT_po_X;
    FFT_po_Y_sum = FFT_po_Y_sum + FFT_po_Y;

    Sa_pred_sum = Sa_pred_sum + Sa_pred  ;
    Sq_pred_sum = Sq_pred_sum + Sq_pred ;

    Sa_po_sum = Sa_po_sum + Sa_po; 
    Sq_po_sum = Sq_po_sum + Sq_po ;

    %% AVERAGE FREQUENCY SPECTRA
    % Used for avweraging many spectras TODO - Incorporate averaging
    FFT_unpo_X_avg = FFT_unpo_X_sum/1;
    FFT_unpo_Y_avg = FFT_unpo_Y_sum/1;

    FFT_pred_X_avg = FFT_pred_X_sum/1;
    FFT_pred_Y_avg = FFT_pred_Y_sum/1;

    FFT_po_X_avg = FFT_po_X_sum/1;
    FFT_po_Y_avg = FFT_po_Y_sum/5;

    Sa_pred_avg = Sa_pred_sum/1;
    Sq_pred_avg = Sq_pred_sum/1;

    Sa_po_avg = Sa_po_sum/1;
    Sq_po_avg = Sq_po_sum/1;

    %round([Sa_pred_avg Sa_poli_avg Sq_pred_avg Sq_poli_avg])

    %% FREQUENCY PLOT COMPARISONS
    if or(ShowGeneralFigures,ShowDebuggingFigures)==1
        % Spatial Frequency in X
        figure
        SoF = 16;
        plot(freqX_unpo,abs(FFT_unpo_X_avg),'-r','linewidth',2.5)
        hold on
        plot(freqX_po,abs(FFT_po_X_avg),'-b','linewidth',2.5)
        plot(freqX_pred,abs(FFT_pred_X_avg),'-g','linewidth',2.5)
        plot([12.5 12.5],[0 0.1],'--k','linewidth',2.5)
        hold off
        legend('Unpolished','Polished','Predicted')
        axis([0 300 0 0.05])
        xlabel('Spatial Frequency f_x (mm^-^1)','FontName','Arial','fontsize',SoF);
        ylabel('Amplitude (\mum)','FontName','Arial','fontsize',SoF);
        set(gca,'FontName','Arial','fontsize',SoF-2);
        annotation('arrow',[0.3125 0.18],[0.645238095238098 0.569047619047622]);
        annotation('textbox',...
            [0.315285714285714 0.62857142857143 0.274 0.0714285714285727],...
            'String',{'f_c = 12.5 mm^-^1'},...
            'FontSize',SoF,...
            'FontName','Arial',...
            'FitBoxToText','off',...
            'LineStyle','none');

        % Spatial Frequency in Y
        figure
        plot(freqY_unpo,abs(FFT_unpo_Y_avg),'-r','linewidth',2.5)
        hold on
        plot(freqY_po,abs(FFT_po_Y_avg),'-b','linewidth',2.5)
        plot(freqY_pred,abs(FFT_pred_Y_avg),'-g','linewidth',2.5)
        plot([12.5 12.5],[0 0.1],'--k','linewidth',2.5);
        hold off
        legend('Unpolished','Polished','Predicted')
        axis([0 300 0 0.05])

        xlabel('Spatial Frequency f_y (mm^-^1)','FontName','Arial','fontsize',SoF);
        ylabel('Amplitude (\mum)','FontName','Arial','fontsize',SoF);
        set(gca,'FontName','Arial','fontsize',SoF-2);
        annotation('arrow',[0.3125 0.18],[0.645238095238098 0.569047619047622]);
        annotation('textbox',...
            [0.315285714285714 0.62857142857143 0.274 0.0714285714285727],...
            'String',{'f_c = 12.5 mm^-^1'},...
            'FontSize',SoF,...
            'FontName','Arial',...
            'FitBoxToText','off',...
            'LineStyle','none');
    end  
    """
