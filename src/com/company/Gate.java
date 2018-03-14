package com.company;

public class Gate {
    public String GateName;
    private Double[][] GateMatrix;
    private int ArgCount;

    Gate(String gateName, int argCount){
        GateName = gateName;
        ArgCount = argCount;
    }

    Gate(String gateName, int argCount, Double[][] gateMatrix) throws Exception{
        GateName = gateName;
        ArgCount = argCount;
        if (MatrixIsValid(gateMatrix))
            GateMatrix = gateMatrix;
    }

    private boolean MatrixIsValid(Double[][] matrix){
        //TODO: write validation if matrix is appropriate for qubit permutation
        return true;
    }


}
