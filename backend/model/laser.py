
class Laser:
    """Plain old python object to hold laser properties."""
    def __init__(self, beam_radius, pulse_duration, avg_power, 
        duty_cycle, pulse_frequency):
        self.beam_radius = beam_radius
        self.pulse_duration = pulse_duration
        self.avg_power = avg_power
        self.duty_cycle = duty_cycle
        self.pulse_frequency = pulse_frequency
        self.pulse_average_power = avg_power / (pulse_frequency * pulse_duration)
