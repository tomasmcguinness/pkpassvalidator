# Apple Wallet Pass Validator

[![Build and deploy .NET Core app to Windows WebApp pkpassvalidator](https://github.com/tomasmcguinness/pkpassvalidator/actions/workflows/pkpassvalidator.yml/badge.svg)](https://github.com/tomasmcguinness/pkpassvalidator/actions/workflows/pkpassvalidator.yml)

# What does this do?
Checks the validity of a pkpass file by checking its signature and contents match the Apple specification. It's hosted on Azure (at my own expense) but I wanted to post the code here, so people can a) see what it does and b) can contribute to the validation it performs.

## Motivation
Questions pop up on StackOverflow about invalid passes and the cause, usually, is a problem in the payload. This project represents my attempt to help developer diagnose the issues themselves. 

## Where is it hosted?
The project is available at https://pkpassvalidator.azurewebsites.net and can be used right now. I'll extend its capabilities over time.

## Support the project
If you find this utility useful and would like to contibute towards the hosting costs, please consider donating by buying me a coffee

<a href='https://ko-fi.com/G2G11TQK5' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://cdn.ko-fi.com/cdn/kofi2.png?v=3' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>


