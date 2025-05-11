from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
import pandas as pd
import uuid
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.naive_bayes import GaussianNB
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import f1_score, precision_score, recall_score, accuracy_score

app = FastAPI()

class KNNDatasetInput(BaseModel):
    data: List[List[str]]
    columns: List[str]
    target_column: str
    neighbors: int

class BayesDatasetInput(BaseModel):
    data: List[List[str]]
    columns: List[str]
    target_column: str

class ModelOutput(BaseModel):
    f1: float
    precision: float
    recall: float
    accuracy: float
    request_id: str

@app.get("/health")
def health_check():
    return {"status": "ok"}
            
@app.post("/knn", response_model=ModelOutput)
async def run_knn(input_data: KNNDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        df = pd.DataFrame(input_data.data, columns=input_data.columns)

        for col in df.columns:
            if col != input_data.target_column:
                df[col] = pd.to_numeric(df[col], errors='coerce')

        df = df.dropna()

        X = df.drop(columns=[input_data.target_column])
        y = df[input_data.target_column]

        le = LabelEncoder()
        y = le.fit_transform(y)

        X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

        knn = KNeighborsClassifier(n_neighbors=input_data.neighbors)
        knn.fit(X_train, y_train)
        y_pred = knn.predict(X_test)

        metrics = {
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "request_id": request_id
        }

        print("KNN Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in KNN:", str(e))
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/bayes", response_model=ModelOutput)
async def run_bayes(input_data: BayesDatasetInput):
    try:
        request_id = str(uuid.uuid4())

        df = pd.DataFrame(input_data.data, columns=input_data.columns)

        for col in df.columns:
            if col != input_data.target_column:
                df[col] = pd.to_numeric(df[col], errors='coerce')

        df = df.dropna()

        X = df.drop(columns=[input_data.target_column])
        y = df[input_data.target_column]

        le = LabelEncoder()
        y = le.fit_transform(y)

        X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

        nb = GaussianNB()
        nb.fit(X_train, y_train)
        y_pred = nb.predict(X_test)

        metrics = {
            "f1": f1_score(y_test, y_pred, average='weighted'),
            "precision": precision_score(y_test, y_pred, average='weighted', zero_division=0),
            "recall": recall_score(y_test, y_pred, average='weighted', zero_division=0),
            "accuracy": accuracy_score(y_test, y_pred),
            "request_id": request_id
        }

        print("Bayes Response:", metrics)

        return ModelOutput(**metrics)

    except Exception as e:
        print("Error in Bayes:", str(e))
        raise HTTPException(status_code=500, detail=str(e))