# Parallel Programming
Complete projects for Parallel Programming class at NRNU MEPhI
# Subject
Create a programm for calculating invertible matrix using cofactors in multithreading mode. The number of threads/proccesses and size of a matrix can be choosen by user.  
Data structure: square matrix  
Dimensions: 10^3 - 10^4 elements  

-----
## Algorithms
Since cofactor of an element ___a___ of a matrix ___A___ is a determinant of some smaller square matrix, cut down from ___A___ by removing one of its rows and columns, multiplied by -1 if element is placed in row _i_ and column _j_ and _i+j_ is an odd number, the programm should be used an efficient algorithm for calculating determinant. For these projects the Bareiss Algorithm was implemented, 'cause of its efficiency and simplicity.

Project 1 - Multithreading  
---------
Each thread calculates a cofactor   
Workflow is managed manually through TaskManager and WorkThread classes  

-----

## Project 2 - Processes  

Each process calculates a cofactor  
When process stops the result value returns to main process to save it in minor matrix  

-----
## Project 3 - TCP/IP connection  

Processes = clients  
When connection opened server sends a matrix to client, then after a positive answer, it starts to exchanges with client tasks (position of cofactor to calculate) and answers. After recieving all answers server calculates an invertible matrix
