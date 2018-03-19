
class Material:
    def __init__(self, name, stc, mu, k, rho_c, T_m, T_b, absp, rho):
        self.name = name
        self.stc = stc
        self.mu = mu
        self.k = k
        self.rho_c = rho_c
        self.T_m = T_m
        self.T_b = T_b
        self.absp = absp
        self.rho = rho
        self.alpha_td = k / rho_c
