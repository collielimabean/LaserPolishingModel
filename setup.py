# -*- coding: utf-8 -*-

from setuptools import setup, find_packages


with open('README.md', 'r', encoding='utf-8') as f:
    readme = f.read()

with open('LICENSE', 'r', encoding='utf-8') as f:
    license = f.read()

setup(
    name='Laser Polishing Model Simulator',
    version='0.0.1',
    description='',
    long_description=readme,
    author='William Jen',
    author_email='wjen@wisc.edu',
    url='https://github.com/collielimabean/LaserPolishingModelSimulator',
    license=license,
    packages=find_packages(exclude=('tests', 'docs'))
)

