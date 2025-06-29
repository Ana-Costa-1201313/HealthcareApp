# US 7.2.13

## 1. Context

This task appears in the end of the project's development, to be able to edit a new specialization.


## 2. Requirements

**US 7.2.11** As an Admin, I want to edit Specializations, so that I can update or correct information about the staff and operation type (procedure). 

**Acceptance Criteria:**

- Admins can add edit specialization with attributes like: 
- Name 
- Description
- The system validates that the name is unique. 


**Dependencies/References:**

It is also required that the user is registered and logged in as an admin.


## 3. Analysis

For this US were considered the requirements specified in the project's description and the client's answers. 
Some relevant answers excerpts are here specified:



```
Q: 

A: 
```


```
Q: 

A: 
```


```
Q: 

A: 
```


```
Q: 

A: 
```



The following **HTTP requests** will be implemented:
- PATCH (to edit a new specialization)


## 4. Design

This section presents the design adopted to solve the requirement.

### 4.1. Sequence Diagram (Level 1)

![SSD_Lvl1.png](SD1.png) 


### 4.2. Sequence Diagram (Level 2)

![SSD_Lvl1.png](SD2.png) 


### 4.3. Sequence Diagram (Level 3)

#### 4.3.1. Backend

![SSD_Lvl1.png](SD3Backend.png) 

#### 4.3.1. Frontend

![SSD_Lvl1.png](SD3Frontend.png) 

### 4.4. Applied Design Patterns

- **Domain Driven Development (DDD):** the focus is the business logic and not the implementation.
- **Data Transfer Object (DTO):** gives an abstraction layer to the domain, so that it's only presented specific information regarding the object.
- **Model View Controller (MVC):** allows the re-usability of components and promotes a more modular approach to the code, making it easier to manage and maintain.
- **Repository pattern:** allows access to data without sharing the details of data storing, like the database connection.
- **Service pattern:** helps keeping high cohesion and low coupling in the code by separating complex business logic from the rest of the system. They also promote reuse, as multiple parts of the system can use the same service to perform common operations.
- **Test Driven Development (TDD):** planning the tests previously to the code gives orientation lines to the development process.
- **Onion Architecture:** concentric layers structure that puts the Domin Model as the core. Promotes modularity, flexibility and testability.
- **Inversion of Control:** the responsability of object creation and dependency management belongs to a framework or external entity, so that the class doesn't need to. Promotes flexibility and decoupling.
- **Dependency Injection:** used to implement inversion of control. The dependencies are injected into a class from the outside.


### 4.5. Tests

#### 4.5.1. Unit and Integration Tests

- Unit Tests for Specialization entity with valid inputs


- Unit tests for the controller focusing on the edition of the Specialization
- Unit tests for the service focusing on the edition of the Specialization
- Unit tests for the repository


#### 4.5.2. Postman Tests

- Status code test
- Response Body Contains fields
- Response Body data validation
- Check if Resource Was edited
- Check if Data is Persisted
- Status Code for Invalid Input and errors

## 5. Implementation

The implementation of this US is according to the design, as can be seen in the SD and CD presented before.

All commits referred the corresponding issue in GitHub, using the #18 and #23 tag, as well as a relevant commit message.



## 6. Integration/Demonstration

To edit a Specialization, run the Backoffice, Auth and SPA app then go to the Specialization page and click on the pencil icon next to a specialization to edit a Specialization.

## 7. Observations

This work was guided by the project provided in ARQSI classes.