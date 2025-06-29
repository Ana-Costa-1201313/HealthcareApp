## 1. Description of the Project

This application is a prototype system for cirurgic requests, appointment and resource management. 

It will allow:
- hospitals and clinics to manage surgery appointments, staff and patient records;
- real-time 3D visualization of resource availability within the facility;
- schedule optimization and resource usage;
- GDPR Regulation (EU) 2016/679 compliance, ensuring the system meets data protection and consent management requirements.

This project is part of the sem5pi-24-25-3db9-backend and sem5pi-24-25-3db9-backfrontend repository, belonging to the sem5pi-24-25-3db9 Project. The solution should be developed in 14 weeks.

## 2. Planning and Technical Documentation

[Planning and Technical Documentation](docs/readme.md)


## 3. How to Build

Run one of the following scripts in the Terminal:
- "build" script, that installs the necessary tools and husky for the pre-commit hook;
- "updateBD" script, that drops the tables and the migrations and creates them again;

*Example:* open the terminal and type ./build.bat

*Note:* This project uses .NET version 8.0 for the Backoffice module and Express for the Records Backoffice module.

To configure the databases:
- for a local database(SQLite), simply run the "build" and the "updateDB" scripts. A .db file will be created in your user folder.
- for the remote DEI database(Microsoft SQL and Mongodb), check the credentials.

*Note:* in the Startup.cs ConfigureServices method, comment the database configuration you are not using, for the Backoffice module.


## 4. How to Execute Tests

Run the "runTests" script, that builds and runs the tests, saying the number of tests and possible failures.

As an alternative, click with the mouse's right button and select "run tests". 


## 5. How to Run

Run the
- "run" script, that builds and runs the application, giving the HTTP/s URLs.
- "runAuth" script, that builds and run the auth module application.
- "runRecordsBO" script, that builds and run the Records Backoffice module.

*Example:* open the terminal and type ./run.bat


**Prolog Server**

To run the Prolog server, needed for the Planning module that plans the operations for a day and the rooms and staff assigned:
- open the SWI console
- consult the file project_operation_room_scheduling3.pl
- type server(8080).

There is also the run_ALGAV.bat script.

## 6. Method of work in the repositories

### 6.1 DoR and DoD

**Definition of Ready**

An User Story is considered ready to start being developed when:
- its dependencies are clarified
- it has been estimated and prioritized appropriately
- it is organized into smaller tasks, if needed
- the acceptance criteria is defined
- the tests are planned


**Definition of Done**

An User Story is considered complete when:
- the code has been written and reviewed
- the tests (unitary, integration, functional, etc) have been written and passed successfully
- documentation is complete and updated


### 6.2 Use of Smart Commits

The User Stories (US) are associated with an issue in the respective GitHub repository. 
When the work is being commited, a relevant commit message should be registered, along with the US identification and the respective issue tag.

***Example:*** US 6.2.12 - Deactivating Staff component. #35


### 6.3  Branches

The code development must be organized by branches, with the following logic:
- Main
- Development
- US specific branch, under the feature folder
- Sprints final releases

The branch flow must follow this order:
 - 1: The new feature branch must be created from the Development, since it's the most up to date branch;
 - 2: When a feature is complete, the Development branch must be merged into the feature branch, to resolve any possible merge conflicts;
 - 3: When the integration is complete, a pull request from that branch into the Development branch should be request;
 - 4: The reviewer accepts the new feature and the feature gets into the Development branch;
 - 5: At the end of the sprint, the Development is merged into the Main branch;
 - 6: A new branch called Sprint X is created from Main at that point, with the release version to be demonstrated at the following presentations.
