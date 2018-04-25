import re
import numpy as np

class ZygoAsciiFile:
    HEADER_REGEX = (r"Zygo ASCII Data File - Format \d+\n"
	r"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.*)\"\n"
	r"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\n"
	r"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\n"
	r"\"(.*)\"\n"
	r"\"(.*)\"\n"
	r"\"(.*)\"\n"
	r"(\d+)\s+(\d+.\d+)\s+(\d+.\de[-]?\d+)\s+(\d+)\s+(\d+)\s(\d+)\s+(\d+.\d+e[-]?\d+)\s+(\d+)\n"
	r"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.*)\"\n"
	r"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+.\d+)\s+(\d+)\s+(\d+)\n"
	r"(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\n"
	r"(\d+)\s+\"(.*)\"\n"
	r"(\d+)\s+(\d+)\n"
	r"\"(.*)\"\n"
	r"#")

    HEADER_DELIMITER = '#'

    HEADER_FIELDS = [
        ('software_type', int),
        ('major_version', int),
        ('minor_version', int),
        ('bug_version', int),
        'software_date',
        ('intens_origin_x', float),
        ('intens_origin_y', float),
        ('intens_width', int),
        ('intens_height', int),
        ('n_buckets', int),
        ('intens_range', float),
        ('phase_origin_x', float),
        ('phase_origin_y', float),
        ('phase_width', int),
        ('phase_height', int),
        'comment',
        'part_serial_number',
        'part_number',
        ('source', int),
        ('intf_scale_factor', float),
        ('wavelength_in', float),
        ('numeric_aperture', float),
        ('obliquity_factor', float),
        ('magnification', float),
        ('camera_resolution', float),
        ('timestamp', int),
        ('camera_width', float),
        ('camera_height', float),
        ('system_type', int),
        ('system_board', int),
        ('system_serial', int),
        ('instrument_id', int),
        'objective_name',
        ('acquire_mode', int),
        ('intensity_averages', int),
        ('pzt_cal', int),
        ('pzt_gain', int),
        ('pzt_gain_tolerance', int),
        ('agc', int),
        ('target_range', float),
        ('light_level', float),
        ('min_mod', int),
        ('min_mod_pts', int),
        ('phase_res', int),
        ('phase_avgs', int),
        ('minimum_area_size', int),
        ('discon_action', int),
        ('discon_filter', float),
        ('connection_order', int),
        ('remove_tilt_bias', int),
        ('data_sign', int),
        ('code_v_type', int),
        ('subtract_sys_err', int),
        'sys_err_file',
        ('refractive_index', float),
        ('part_thickness', float),
        'zoom_desc' 
    ]

    def __init__(self, filename):
        self.load_file(filename)

    def load_file(self, filename):
        with open(filename, 'r') as f:
            raw_text = f.read()

            match = re.match(self.HEADER_REGEX, raw_text)
            if not match:
                raise Exception('File does not have valid Zygo ASCII header!')

            groups = match.groups()
            if len(groups) != len(self.HEADER_FIELDS):
                raise Exception('File does not have valid Zygo ASCII header!')

            for header_field, group in zip(self.HEADER_FIELDS, groups):
                if isinstance(header_field, tuple):
                    field = header_field[0]
                    header_type = header_field[1]
                    self.__dict__[field] = header_type(group.strip())
                else:
                    self.__dict__[header_field] = group.strip()

            if self.n_buckets != 1:
                raise Exception('> 1 bucket not supported')

            self.phase_res = 32768 if self.phase_res == 1 else 4096
            data = raw_text.split('#')
            self.intensity_data = self._parse_int_array(data[1], self.intens_height, self.intens_width, 65535)

            phase_data_scale_factor = self.intf_scale_factor * self.wavelength_in * self.obliquity_factor / self.phase_res
            self.phase_data = self._parse_int_array(data[2], self.phase_height, self.phase_width, 2147483640, phase_data_scale_factor)

    def _parse_int_array(self, raw_array, height, width, invalid_threshold, scale_factor = 1):
        parsed_array = np.fromstring(raw_array, dtype=float, sep=' ')
        parsed_array[parsed_array > invalid_threshold] = -1
        return np.reshape(parsed_array, [height, width]) * scale_factor

    def __str__(self):
        return str(self.__dict__)

if __name__ == "__main__":
    a = ZygoAsciiFile("C:\\Users\\William\\Documents\\Sensitivity Edit - Brodan\\1-15-15_MilledH_1_20x.asc")
    print(str(a))
    print(np.shape(a.intensity_data))
