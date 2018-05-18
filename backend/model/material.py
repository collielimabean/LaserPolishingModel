
class Material:
    """ Plain old python object to store material data."""
    def __init__(self, stc, mu, k, rho_c, T_m, T_b, absp, rho):
        self.stc = stc
        self.mu = mu
        self.k = k
        self.rho_c = rho_c
        self.T_m = T_m
        self.T_b = T_b
        self.absp = absp
        self.rho = rho
        self.alpha_td = k / rho_c
