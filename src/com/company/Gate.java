package com.company;

public class Gate {
    private String GateName;
    private Double[][] GateMatrix;
    private Integer ArgCount;

    public String getGateName() { return GateName;}
    public Integer getArgCount() { return ArgCount;}
    public Double[][] getGateMatrix() { return GateMatrix;}

    public void setGateName(String gateName) { GateName = gateName;}
    public void setArgCount(Integer argCount) { ArgCount = argCount;}
    public void setGateMatrix(Double[][] gateMatrix) { GateMatrix = gateMatrix;}

    public Gate(String gateName, Integer argCount){
        GateName = gateName;
        ArgCount = argCount;
    }

    public Gate(String gateName, Integer argCount, Double[][] gateMatrix) throws Exception{
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
