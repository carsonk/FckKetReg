# FckKetReg #
*(Current Version: 1.0-Beta)*

*FrolickingKettlesRegiment* (or, *FckKetReg*) is a bot for Kettering University's registration system. It will log into Kettering's Banner Web via provided credentials, pass it a term and PIN to register with, and then register for classes with CRNs provided by the user. 

*FckKetReg*'s bot can also be scheduled to fire at a specific time. At that time, it will check login credentials, and then try to reach the registration page. If the bot fails to reach the registration page, it will try another 60 times over the following minute. This is to account for any possible discrepencies between the local box's time and the time on Kettering's Banner server.

**WARNING: Your computer must be left on and FckKetReg must be left running for the job to fire at the correct time.**

## Download ##

Installer can be downloaded from [SourceForge](http://sourceforge.net/projects/fckketreg/files/latest/download).

## Instructions for Scheduling Registration ##

- Open FckKetReg.
- Enter LDAP username and password. 
- Enter the registration PIN given to you by your adviser.
- Choose the term that you want to register for.
- Ensure that your login credentials are correct by pressing the "Check Login" button.
- Enter the CRNs in the order that they should be submitted to the server. These boxes will submit in the same way as the boxes on the Add/Drop page.
- Set the time that you want the bot to fire. Note that if *FckKetReg* fails to reach the registration page at this time, it will try to fire another 60 times over the next minute.
- Once the job is completed, *FckKetReg* will pop a message box showing the status of the registration.

## Known Pitfalls ##

- Currently does not show HTML preview from scheduled operation. It *will* show for run that is manually fired since it is run on the same thread. This needs to be fixed using Quartz's TriggerListener.
- Currently will not show if user had an invalid CRN. This can be done via sampling.
