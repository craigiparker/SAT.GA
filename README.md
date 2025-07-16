# SAT Solver Using Genetic Algorithms

## Overview

This project implements a stochastic SAT solver using Genetic Algorithms (GAs) to solve Boolean satisfiability problems in Conjunctive Normal Form (CNF). The solver explores different evolutionary strategies and hybrid approaches to efficiently navigate the solution space of SAT problems.

Key features:
- Pure GA implementation with configurable operators (selection, crossover, mutation)
- Hybrid methods including local search (Tabu, Hill Climbing, Simulated Annealing)
- Parallel execution support for improved performance
- DIMACS CNF file format compatibility
- Comprehensive benchmarking and evaluation framework

## Getting Started

### Prerequisites
- .NET Core 3.1 or later
- Any OS (Windows/Linux/macOS)

### Installation
1. Clone the repository:
   ```
   git clone https://github.com/craigiparker/SAT.GA.git
   ```
2. Build the project:
   ```
   cd SAT.GA
   dotnet build
   ```

### Usage
Run the solver with a CNF file or directory of CNF files:
```
SAT.GA <input.cnf> [options]
```

#### Basic Example
```
SAT.GA benchmarks/uf20-01.cnf
```

#### With Custom Parameters
```
SAT.GA benchmarks/uf50-218.cnf -p 50 -g 1000 -m 0.02 -tc 4
```

### Command Line Options

| Option               | Description                          | Default |
|----------------------|--------------------------------------|---------|
| `-p`, `-population`  | Population size                      | 100     |
| `-g`, `-generations` | Maximum generations                  | 500     |
| `-m`, `-mutation-rate` | Mutation probability              | 0.01    |
| `-c`, `-crossover-rate` | Crossover probability            | 0.9     |
| `-t`, `-tournament`  | Tournament size for selection        | 3       |
| `-tc`, `-thread-count` | Number of parallel threads        | 1       |
| `-s`, `-seed`        | Random seed for reproducibility      | (none)  |
| `-h`, `-help`        | Show help message                    |         |

Full list of available options can be viewed with `-help`.

## Benchmark Problems

The solver has been tested on:
- Uniform Random 3-SAT instances (20-250 variables)
- Structured graph coloring problems (90-300 variables)

Sample benchmarks are included in the `benchmarks` directory.

## Output Format

The solver outputs either:
```
SATISFIABLE
<solution as space-separated literals>
```
or
```
TIMEOUT
```

Runtime statistics are also displayed including:
- Number of generations
- Restarts performed
- Elapsed time
- Best fitness achieved


## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Contact

For questions or feedback, please contact the author.
